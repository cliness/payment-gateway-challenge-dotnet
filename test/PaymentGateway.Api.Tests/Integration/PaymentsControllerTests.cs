using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Payments;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Tests.Integration;

public class PaymentsControllerTests
{
    [Fact]
    public async Task MakesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = 2030,
            ExpiryMonth = 4,
            Amount = 1000,
            CardNumberLastFour = 3487,
            Currency = "GBP"
        };

        var paymentsRepository = new InMemoryPaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", payment);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
    }

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new CardPayment
        {
            Id = Guid.NewGuid(),
            ExpiryYear = 2023,
            ExpiryMonth =8,
            Amount = 900,
            CardNumber = 2345,
            Currency = "GBP"
        };

        var paymentsRepository = new InMemoryPaymentsRepository();
        paymentsRepository.AddOrUpdate(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}