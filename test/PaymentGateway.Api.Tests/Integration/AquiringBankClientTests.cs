﻿using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using PaymentGateway.Api.Models.AquiringBank;
using PaymentGateway.Api.Tests.Configuration;
using PaymentGateway.Api.Infrastructure;
using Moq;

namespace PaymentGateway.Api.Tests.Integration
{
    [Trait("Category", "ThirdPartyAPI")]
    public class AquiringBankClientTests
    {
        private readonly Uri _acquiringPaymentEndpoint;

        public AquiringBankClientTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            var acquiringPayment = config.GetRequiredSection(AquiringPaymentSettings.Section).Get<AquiringPaymentSettings>();
            if(acquiringPayment?.ServiceEndpoint == null)
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

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryDate = "04/2025",
                Currency = "GBP",
                Amount = 100,                
            };
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var aquiringBankClient = new AquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await aquiringBankClient.PostPayment(paymentRequest);

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

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 2222405343248112,
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var aquiringBankClient = new AquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await aquiringBankClient.PostPayment(paymentRequest);

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

            var paymentRequest = new AquiringBankPaymentRequest
            {
                CardNumber = 1111111111111111,
                ExpiryDate = "01/2026",
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var aquiringBankClient = new AquiringBankClient(httpClientFactory.Object);

            //Act
            var paymentResponse = await aquiringBankClient.PostPayment(paymentRequest);

            //Assert
            Assert.NotNull(paymentResponse);
            Assert.NotNull(paymentResponse?.ErrorMessage);
        }
    }
}
