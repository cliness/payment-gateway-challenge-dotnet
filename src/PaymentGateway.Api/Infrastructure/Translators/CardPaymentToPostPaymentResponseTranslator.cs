using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;

namespace PaymentGateway.Api.Infrastructure.Translators
{
    public static class CardPaymentToPostPaymentResponseTranslator
    {
        public static PostPaymentResponse ToPostPaymentResponse(this CardPayment cardPayment)
        {
            return new PostPaymentResponse
            {
                Id = cardPayment.Id,
                Status = cardPayment.Status,
                CardNumberLastFour = cardPayment.CardNumber.Substring(cardPayment.CardNumber.Length - 4, 4),
                ExpiryMonth = cardPayment.ExpiryMonth,
                ExpiryYear = cardPayment.ExpiryYear,
                Amount = cardPayment.Amount,
                Currency = cardPayment.Currency,
            };
        }
    }
}
