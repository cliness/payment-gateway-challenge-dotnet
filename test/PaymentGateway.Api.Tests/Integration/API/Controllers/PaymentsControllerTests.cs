using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Api.Controllers;
using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Configuration;
using PaymentGateway.Api.Infrastructure.Providers;
using PaymentGateway.Api.Infrastructure.Repository;

namespace PaymentGateway.Api.Tests.Integration.Api.Controllers;

[Trait("Category", "ThirdPartyAPI")]
public class PaymentsControllerTests
{
    private readonly string[] _validCurrencyCodes;

    public PaymentsControllerTests()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

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

        var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        serializeOptions.Converters.Add(new JsonStringEnumConverter());

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(dateProviderMock.Object)))
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", payment);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(serializeOptions);
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

        var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        serializeOptions.Converters.Add(new JsonStringEnumConverter());

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(dateProviderMock.Object)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>(serializeOptions);
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
                .AddSingleton<IPaymentValidatorService>(services => new PaymentValidatorService(services.GetRequiredService<IDateProvider>(), _validCurrencyCodes))
                .AddSingleton(new Mock<IDateProvider>().Object)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}