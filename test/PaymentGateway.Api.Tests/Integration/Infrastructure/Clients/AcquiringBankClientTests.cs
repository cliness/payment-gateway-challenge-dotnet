using System.Text.Json;

using Microsoft.Extensions.Configuration;

using Moq;

using PaymentGateway.Api.Infrastructure.AcquiringBank;
using PaymentGateway.Api.Infrastructure.Configuration;
using PaymentGateway.Api.Infrastructure.Models;

namespace PaymentGateway.Api.Tests.Integration.Infrastructure.Clients
{
    [Trait("Category", "ThirdPartyAPI")]
    public class AcquiringBankClientTests
    {
        private readonly Uri _acquiringPaymentEndpoint;

        public AcquiringBankClientTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            var acquiringPayment = config.GetRequiredSection(nameof(AcquiringPaymentSettings)).Get<AcquiringPaymentSettings>();
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

            var paymentRequest = new AcquiringBankPaymentRequest
            {
                CardNumber = "2222405343248877",
                Cvv = "123",
                ExpiryDate = "04/2025",
                Currency = "GBP",
                Amount = 100,
            };
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var acquiringBankClient = new AcquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await acquiringBankClient.PostPayment(paymentRequest);

            //Assert
            Assert.NotNull(paymentResponse);
            Assert.True(paymentResponse.Authorized);
            Assert.NotNull(paymentResponse.AuthorizationCode);
        }

        [Fact]
        public async Task Payment_NotAuthorisedCard_ReturnsNotAuthorisedPayment()
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

            var paymentRequest = new AcquiringBankPaymentRequest
            {
                CardNumber = "2222405343248112",
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var acquiringBankClient = new AcquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await acquiringBankClient.PostPayment(paymentRequest);

            //Assert
            Assert.NotNull(paymentResponse);
            Assert.False(paymentResponse.Authorized);
            Assert.Null(paymentResponse.AuthorizationCode);
        }

        [Fact]
        public async Task Payment_UnknownCard_ReturnsNotAuthorisedPayment()
        {
            //Arrange
            var httpClient = new HttpClient()
            {
                BaseAddress = _acquiringPaymentEndpoint
            };
            var requestSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            var errorSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var paymentRequest = new AcquiringBankPaymentRequest
            {
                CardNumber = "1111111111111111",
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var acquiringBankClient = new AcquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await acquiringBankClient.PostPayment(paymentRequest);

            //Assert
            Assert.NotNull(paymentResponse);
            Assert.NotNull(paymentResponse?.ErrorMessage);
        }
    }
}

