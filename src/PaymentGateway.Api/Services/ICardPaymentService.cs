using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Services
{
    public interface ICardPaymentService
    {
        Task<CardPayment> MakePayment(CardPayment payment);
    }
}