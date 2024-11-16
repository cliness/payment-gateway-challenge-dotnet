using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Infrastructure.Masking;

namespace PaymentGateway.Api.Infrastructure.Translators
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
                Status = payment.Status
            };
        }
    }
}
