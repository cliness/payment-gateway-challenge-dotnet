using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Infrastructure.Masking;

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
                CardNumberLastFour = cardPayment.CardNumber.ToLastFourDigits(),
                ExpiryMonth = cardPayment.ExpiryMonth,
                ExpiryYear = cardPayment.ExpiryYear,
                Amount = cardPayment.Amount,
                Currency = cardPayment.Currency,
            };
        }
    }
}
