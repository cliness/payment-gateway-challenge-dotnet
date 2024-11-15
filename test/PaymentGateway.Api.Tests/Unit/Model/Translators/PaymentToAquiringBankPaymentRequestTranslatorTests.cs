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
                Amount = 10,
                CardNumber = 2222405343248112,
                AuthorizationCode = Guid.NewGuid().ToString(),
                Currency = "GDP",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2026,
                Id = Guid.NewGuid(),
                Status = Models.PaymentStatus.Authorized,
            };

            var aquiringBankPaymentRequest = cardPayment.ToAquiringBankPaymentRequest();

            Assert.Equal(10, aquiringBankPaymentRequest.Amount);
            Assert.Equal(2222405343248112, aquiringBankPaymentRequest.CardNumber);
            Assert.Equal("GDP", aquiringBankPaymentRequest.Currency);
            Assert.Equal("04/2026", aquiringBankPaymentRequest.ExpiryDate);
            Assert.Equal("123", aquiringBankPaymentRequest.Cvv);
        }
    }
}
