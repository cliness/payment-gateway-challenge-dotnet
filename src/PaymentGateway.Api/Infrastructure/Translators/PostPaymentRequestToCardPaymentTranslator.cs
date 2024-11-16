using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Infrastructure.Translators
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
