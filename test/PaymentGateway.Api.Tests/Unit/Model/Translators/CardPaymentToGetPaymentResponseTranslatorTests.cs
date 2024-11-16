using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;

namespace PaymentGateway.Api.Tests.Unit.Model.Translators
{
    public class CardPaymentToGetPaymentResponseTranslatorTests
    {
        [Fact]
        public void ToGetPaymentResponse_CardPayment_TranslatesToResponce()
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
            var acquiringBankPaymentRequest = cardPayment.ToGetPaymentResponse();

            //Assert
            Assert.Equal(8877, acquiringBankPaymentRequest.CardNumberLastFour);            
            Assert.Equal(2025, acquiringBankPaymentRequest.ExpiryYear);
            Assert.Equal(4, acquiringBankPaymentRequest.ExpiryMonth);

            Assert.Equal(100, acquiringBankPaymentRequest.Amount);
            Assert.Equal("GBP", acquiringBankPaymentRequest.Currency);

            Assert.Equal(PaymentStatus.Authorized, acquiringBankPaymentRequest.Status);
        }
    }
}
