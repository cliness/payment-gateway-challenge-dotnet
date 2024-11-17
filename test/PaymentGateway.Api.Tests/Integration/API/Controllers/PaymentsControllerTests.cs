using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Api.Api.Controllers;
using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Infrastructure.AcquiringBank;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Repository;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Infrastructure.Configuration;
using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Infrastructure.Providers;
using Moq;

namespace PaymentGateway.Api.Tests.Integration.Api.Controllers;

[Trait("Category", "ThirdPartyAPI")]
public class PaymentsControllerTests
{
    private readonly Uri _acquiringPaymentEndpoint;
    private readonly string[] _validCurrencyCodes;

    public PaymentsControllerTests()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        var acquiringPayment = config.GetRequiredSection(nameof(AcquiringBankPaymentSettings)).Get<AcquiringBankPaymentSettings>();
        if (acquiringPayment?.ServiceEndpoint == null)
        {
            throw new Exception("Acquiring Service Endpoint not defined");
        }
        _acquiringPaymentEndpoint = acquiringPayment.ServiceEndpoint;

        var paymentGatewaySettings = config.GetRequiredSection(nameof(PaymentGatewaySettings)).Get<PaymentGatewaySettings>();
        if (paymentGatewaySettings == null || !paymentGatewaySettings.ValidCurrencyCodes.Any())
        {
            throw new Exception("Payment Gateway Valid Currency Codes not defined");
        }
        _validCurrencyCodes = paymentGatewaySettings.ValidCurrencyCodes;
    }

    [Fact]
    public async Task MakePayment_ValidCard_MakesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            Cvv = "123",
            ExpiryYear = 2025,
            ExpiryMonth = 4,
            Amount = 100,
            Currency = "GBP"
        };

        var paymentsRepository = new InMemoryPaymentsRepository();

        var dateProviderMock = new Mock<IDateProvider>();
        dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<ICardPaymentService, CardPaymentService>()
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(dateProviderMock.Object)
                .AddSingleton<IAcquiringBankClient, AcquiringBankClient>()
                .AddHttpClient(nameof(AcquiringBankClient), client =>
                {
                    client.BaseAddress = _acquiringPaymentEndpoint;
                })))
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", payment);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
    }

    [Fact]
    public async Task GetPayment_WithCardPayment_RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new CardPayment
        {
            Id = Guid.NewGuid(),

            CardNumber = "2222405343248877",
            Cvv = "123",
            ExpiryYear = 2025,
            ExpiryMonth = 4,

            Amount = 100,
            Currency = "GBP",

            Status = PaymentStatus.Authorized
        };

        var paymentsRepository = new InMemoryPaymentsRepository();
        paymentsRepository.AddOrUpdate(payment);

        var dateProviderMock = new Mock<IDateProvider>();
        dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<ICardPaymentService, CardPaymentService>()
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(dateProviderMock.Object)
                .AddSingleton<IAcquiringBankClient, AcquiringBankClient>()
                .AddHttpClient(nameof(AcquiringBankClient), client =>
                {
                    client.BaseAddress = _acquiringPaymentEndpoint;
                })))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task GetPayment_WithoutCardPayment_Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>()
                .AddSingleton<ICardPaymentService, CardPaymentService>()
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(new Mock<IDateProvider>().Object)
                .AddSingleton<IAcquiringBankClient, AcquiringBankClient>()
                .AddHttpClient(nameof(AcquiringBankClient), client =>
                {
                    client.BaseAddress = _acquiringPaymentEndpoint;
                })))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}