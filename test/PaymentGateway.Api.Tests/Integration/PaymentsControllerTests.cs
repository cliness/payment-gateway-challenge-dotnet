using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Payments;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Tests.Configuration;

namespace PaymentGateway.Api.Tests.Integration;

public class PaymentsControllerTests
{
    private readonly Uri _acquiringPaymentEndpoint;

    public PaymentsControllerTests()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        var acquiringPayment = config.GetRequiredSection(AquiringPaymentSettings.Section).Get<AquiringPaymentSettings>();
        if (acquiringPayment?.ServiceEndpoint == null)
        {
            throw new Exception("Acquiring Service Endpoint not defined");
        }
        _acquiringPaymentEndpoint = acquiringPayment.ServiceEndpoint;
    }

    [Fact]
    public async Task MakePayment_ValidCard_MakesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            CardNumber = 2222405343248877,
            Cvv = "123",
            ExpiryYear = 2025,
            ExpiryMonth = 4,
            Amount = 100,            
            Currency = "GBP"
        };

        var paymentsRepository = new InMemoryPaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<ICardPaymentService, CardPaymentService>()
                .AddHttpClient(nameof(CardPaymentService), client =>
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

            CardNumber = 2222405343248877,
            Cvv = "123",
            ExpiryYear = 2025,
            ExpiryMonth = 4,

            Amount = 100,            
            Currency = "GBP",

            Status = PaymentStatus.Authorized
        };

        var paymentsRepository = new InMemoryPaymentsRepository();
        paymentsRepository.AddOrUpdate(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddSingleton<ICardPaymentService, CardPaymentService>()
                .AddHttpClient(nameof(CardPaymentService), client =>
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
                .AddHttpClient(nameof(CardPaymentService), client =>
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