using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;

namespace PaymentGateway.Api.Infrastructure.Translators
{
    public static class CardPaymentToGetPaymentResponseTranslator
    {
        public static GetPaymentResponse ToGetPaymentResponse(this CardPayment payment)
        {
            return new GetPaymentResponse
            {
                Id = payment.Id,
                CardNumberLastFour = payment.CardNumber.Substring(payment.CardNumber.Length - 4, 4),
                Amount = payment.Amount,
                Currency = payment.Currency,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                Status = payment.Status
            };
        }
    }
}
