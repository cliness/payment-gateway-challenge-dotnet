using PaymentGateway.Api.Domain.CardPayments;

namespace PaymentGateway.Api.Domain.Services
{
    public interface ICardPaymentService
    {
        Task<CardPayment> MakePayment(CardPayment payment);
    }
}