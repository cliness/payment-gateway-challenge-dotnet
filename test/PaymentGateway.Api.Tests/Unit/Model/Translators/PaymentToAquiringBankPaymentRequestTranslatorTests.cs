using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;

namespace PaymentGateway.Api.Tests.Unit.Model.Translators
{
    public class PaymentToAquiringBankPaymentRequestTranslatorTests
    {
        [Fact]
        public void ToAquiringBankPaymentRequest_CardPayment_TranslatesToRequest()
        {
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),

                CardNumber = 2222405343248112,                
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2026,

                Amount = 100,
                Currency = "GBP",

                AuthorizationCode = Guid.NewGuid().ToString(),
                Status = Models.PaymentStatus.Authorized,
            };

            var aquiringBankPaymentRequest = cardPayment.ToAquiringBankPaymentRequest();
            
            Assert.Equal(2222405343248112, aquiringBankPaymentRequest.CardNumber);
            Assert.Equal("04/2026", aquiringBankPaymentRequest.ExpiryDate);
            Assert.Equal("123", aquiringBankPaymentRequest.Cvv);
            Assert.Equal(100, aquiringBankPaymentRequest.Amount);
            Assert.Equal("GBP", aquiringBankPaymentRequest.Currency);            
        }
    }
}
