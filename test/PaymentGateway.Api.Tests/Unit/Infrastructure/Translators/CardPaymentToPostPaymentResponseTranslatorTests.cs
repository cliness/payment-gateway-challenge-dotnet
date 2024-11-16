using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Infrastructure.Translators;

namespace PaymentGateway.Api.Tests.Unit.Infrastructure.Translators
{
    public class CardPaymentToPostPaymentResponseTranslatorTests
    {
        [Fact]
        public void ToPostPaymentResponse_CardPayment_TranslatesToResponse()
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),

                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,

                Amount = 100,
                Currency = "GBP",

                AuthorizationCode = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
            };

            //Act
            var postPaymentResponse = cardPayment.ToPostPaymentResponse();

            //Assert
            Assert.Equal(cardPayment.Id, postPaymentResponse.Id);

            Assert.Equal(8877, postPaymentResponse.CardNumberLastFour);

            Assert.Equal(2025, postPaymentResponse.ExpiryYear);
            Assert.Equal(4, postPaymentResponse.ExpiryMonth);
            Assert.Equal(PaymentStatus.Authorized, postPaymentResponse.Status);

            Assert.Equal(100, postPaymentResponse.Amount);
            Assert.Equal("GBP", postPaymentResponse.Currency);
        }
    }
}
