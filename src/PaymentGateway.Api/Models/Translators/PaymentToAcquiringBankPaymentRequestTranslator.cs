using PaymentGateway.Api.Models.AcquiringBank;
using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Models.Translators
{
    public static class PaymentToAcquiringBankPaymentRequestTranslator
    {
        public static AcquiringBankPaymentRequest ToAcquiringBankPaymentRequest(this CardPayment payment)
        {
            return new AcquiringBankPaymentRequest
            {
                CardNumber = payment.CardNumber,
                Cvv = payment.Cvv,
                ExpiryDate = $"{payment.ExpiryMonth:00}/{payment.ExpiryYear}",
                Amount = payment.Amount,
                Currency = payment.Currency,
            };
        }
    }
}
