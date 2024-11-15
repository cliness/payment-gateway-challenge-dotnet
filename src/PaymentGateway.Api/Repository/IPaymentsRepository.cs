using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Repository
{
    public interface IPaymentsRepository
    {
        void AddOrUpdate(CardPayment payment);
        CardPayment? Get(Guid id);
    }
}