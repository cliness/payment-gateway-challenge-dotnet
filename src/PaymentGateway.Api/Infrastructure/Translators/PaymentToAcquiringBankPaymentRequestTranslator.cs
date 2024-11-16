using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Infrastructure.Models;

namespace PaymentGateway.Api.Infrastructure.Translators
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
