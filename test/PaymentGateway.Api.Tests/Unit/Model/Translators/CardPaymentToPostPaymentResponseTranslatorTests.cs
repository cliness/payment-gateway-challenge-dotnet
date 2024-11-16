using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Tests.Unit.Model.Translators
{
    public class CardPaymentToPostPaymentResponseTranslatorTests
    {
        public void ToPostPaymentResponse()
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),

                CardNumber = 2222405343248112,
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
            
            Assert.Equal(8112, postPaymentResponse.CardNumberLastFour);
            
            Assert.Equal(2025, postPaymentResponse.ExpiryYear);
            Assert.Equal(4, postPaymentResponse.ExpiryMonth);
            Assert.Equal(PaymentStatus.Authorized, postPaymentResponse.Status);

            Assert.Equal(100, postPaymentResponse.Amount);
            Assert.Equal("GBP", postPaymentResponse.Currency);
        }
    }
}
