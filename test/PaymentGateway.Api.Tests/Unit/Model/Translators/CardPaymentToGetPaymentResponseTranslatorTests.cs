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

            var aquiringBankPaymentRequest = cardPayment.ToGetPaymentResponse();

            Assert.Equal(10, aquiringBankPaymentRequest.Amount);
            Assert.Equal(8112, aquiringBankPaymentRequest.CardNumberLastFour);
            Assert.Equal("GDP", aquiringBankPaymentRequest.Currency);
            Assert.Equal(2026, aquiringBankPaymentRequest.ExpiryYear);
            Assert.Equal(4, aquiringBankPaymentRequest.ExpiryMonth);
            Assert.Equal(PaymentStatus.Authorized, aquiringBankPaymentRequest.Status);
        }
    }
}
