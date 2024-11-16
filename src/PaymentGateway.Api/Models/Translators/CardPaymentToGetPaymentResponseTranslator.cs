using PaymentGateway.Api.Masking;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Payments;

namespace PaymentGateway.Api.Models.Translators
{
    public static class CardPaymentToGetPaymentResponseTranslator
    {
        public static GetPaymentResponse ToGetPaymentResponse(this CardPayment payment)
        {
            return new GetPaymentResponse
            {
                Id = payment.Id,
                CardNumberLastFour = payment.CardNumber.ToLastFourDigits(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,                
                Status= payment.Status
            };
        }
    }
}
