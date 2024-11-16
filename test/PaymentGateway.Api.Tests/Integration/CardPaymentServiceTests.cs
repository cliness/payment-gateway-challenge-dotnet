using System.Text.Json;

using Microsoft.Extensions.Configuration;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Tests.Configuration;

namespace PaymentGateway.Api.Tests.Integration
{
    public class CardPaymentServiceTests
    {
        private readonly Uri _acquiringPaymentEndpoint;

        public CardPaymentServiceTests()
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
        public async Task Payment_AuthorisedCard_ReturnsAuthorisedPayment()
        {
            //Arrange
            var httpClient = new HttpClient()
            {
                BaseAddress = _acquiringPaymentEndpoint
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            var cardPayment = new CardPayment
            {
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
            };
            InMemoryPaymentsRepository paymentsRepository = new InMemoryPaymentsRepository();
            var cardPaymentService = new CardPaymentService(httpClient, paymentsRepository);

            //Act
            var returnedCardPayment = await cardPaymentService.MakePayment(cardPayment);

            //Assert
            Assert.NotNull(returnedCardPayment);
            Assert.Equal(PaymentStatus.Authorized, returnedCardPayment.Status);
            Assert.NotNull(returnedCardPayment.AuthorizationCode);

            Assert.Equal(cardPayment.Id, returnedCardPayment.Id);            
            Assert.Equal(2222405343248877, returnedCardPayment.CardNumber);            
            Assert.Equal(2025, returnedCardPayment.ExpiryYear);
            Assert.Equal(4, returnedCardPayment.ExpiryMonth);

            Assert.Equal(100, returnedCardPayment.Amount);
            Assert.Equal("GBP", returnedCardPayment.Currency);
        }
    }
}
