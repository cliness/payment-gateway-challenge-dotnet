using PaymentGateway.Api.Domain.CardPayments;

namespace PaymentGateway.Api.Domain.Services
{
    public interface IPaymentValidatorService
    {
        bool IsNotValid(CardPayment payment);
    }
}