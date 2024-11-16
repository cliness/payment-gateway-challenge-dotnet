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
                
                CardNumber = 2222405343248112,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,

                Amount = 100,
                Currency = "GBP",

                AuthorizationCode = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Authorized,
            };

            //Act
            var aquiringBankPaymentRequest = cardPayment.ToGetPaymentResponse();

            //Assert
            Assert.Equal(8112, aquiringBankPaymentRequest.CardNumberLastFour);            
            Assert.Equal(2025, aquiringBankPaymentRequest.ExpiryYear);
            Assert.Equal(4, aquiringBankPaymentRequest.ExpiryMonth);

            Assert.Equal(100, aquiringBankPaymentRequest.Amount);
            Assert.Equal("GBP", aquiringBankPaymentRequest.Currency);

            Assert.Equal(PaymentStatus.Authorized, aquiringBankPaymentRequest.Status);
        }
    }
}
