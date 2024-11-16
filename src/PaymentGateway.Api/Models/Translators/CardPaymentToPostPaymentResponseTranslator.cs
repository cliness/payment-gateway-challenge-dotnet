using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Payments;

namespace PaymentGateway.Api.Tests.Unit.Model.Translators
{
    public static class CardPaymentToPostPaymentResponseTranslator
    {
        public static PostPaymentResponse ToPostPaymentResponse(this CardPayment cardPayment)
        {
            return new PostPaymentResponse
            {
                Id = cardPayment.Id,
                Status = cardPayment.Status,
                CardNumberLastFour = (int)cardPayment.CardNumber % 1000,
                ExpiryMonth = cardPayment.ExpiryMonth,
                ExpiryYear = cardPayment.ExpiryYear,
                Amount = cardPayment.Amount,
                Currency = cardPayment.Currency,                
            };
        }
    }
}
