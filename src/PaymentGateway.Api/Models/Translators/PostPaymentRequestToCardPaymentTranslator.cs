using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Payments;

namespace PaymentGateway.Api.Models.Translators
{
    public static class PostPaymentRequestToCardPaymentTranslator
    {
        public static CardPayment ToCardPayment(this PostPaymentRequest paymentRequest, PaymentStatus paymentStatus, Guid id)
        {
            return new CardPayment
            {
                Id = id,
                Status = paymentStatus,

                CardNumber = paymentRequest.CardNumber,
                Cvv = paymentRequest.Cvv,
                ExpiryMonth = paymentRequest.ExpiryMonth,
                ExpiryYear = paymentRequest.ExpiryYear,

                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
            };
        }
    }
}
