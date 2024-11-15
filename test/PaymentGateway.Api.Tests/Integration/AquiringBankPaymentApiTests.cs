using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Tests.Configuration;

namespace PaymentGateway.Api.Tests.Integration
{
    [Trait("Category", "ThirdPartyIntegration")]
    public class AquiringBankPaymentApiTests
    {
        private readonly Uri acquiringPaymentEndpoint;

        public AquiringBankPaymentApiTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            var acquiringPayment = config.GetRequiredSection(AquiringPaymentSettings.Section).Get<AquiringPaymentSettings>();
            if(acquiringPayment?.ServiceEndpoint == null)
            {
                throw new Exception("Acquiring Service Endpoint not defined");
            }
            acquiringPaymentEndpoint = acquiringPayment.ServiceEndpoint;
        }

        [Fact]
        public async Task Payment_AuthorisedCard_ReturnsAuthorisedPayment()
        {
            //Arrange
            var httpClient = new HttpClient();
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 2222405343248877,
                ExpiryDate = "04/2025",
                Currency = "GBP",
                Amount = 100,
                Cvv = "123"
            };

            //Act
            var httpResponse = await httpClient.PostAsJsonAsync(acquiringPaymentEndpoint, paymentRequest, serializeOptions);

            //Assert
            httpResponse.EnsureSuccessStatusCode();

            var paymentResponse = await httpResponse.Content.ReadFromJsonAsync<AquiringBankPaymentResponse>(serializeOptions);

            Assert.NotNull(paymentResponse);
            Assert.True(paymentResponse.Authorized);
            Assert.NotEmpty(paymentResponse.AuthorizationCode);
        }

        [Fact]
        public async Task Payment_NotAuthorisedCard_ReturnsNotAuthorisedPayment()
        {
            //Arrange
            var httpClient = new HttpClient();
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 2222405343248112,
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            //Act
            var httpResponse = await httpClient.PostAsJsonAsync(acquiringPaymentEndpoint, paymentRequest, serializeOptions);

            //Assert
            httpResponse.EnsureSuccessStatusCode();

            var paymentResponse = await httpResponse.Content.ReadFromJsonAsync<AquiringBankPaymentResponse>(serializeOptions);

            Assert.NotNull(paymentResponse);
            Assert.False(paymentResponse.Authorized);
            Assert.Empty(paymentResponse.AuthorizationCode);
        }

        [Fact]
        public async Task Payment_UnknownCard_ReturnsNotAuthorisedPayment()
        {
            //Arrange
            var httpClient = new HttpClient();
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

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 1111111111111111,
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            //Act
            var httpResponse = await httpClient.PostAsJsonAsync(acquiringPaymentEndpoint, paymentRequest, requestSerializeOptions);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

            var paymentResponse = await httpResponse.Content.ReadFromJsonAsync<AquiringPaymentErrorResponse>();
            Assert.NotNull(paymentResponse);
            Assert.NotEmpty(paymentResponse.ErrorMessage);

        }
    }
}

