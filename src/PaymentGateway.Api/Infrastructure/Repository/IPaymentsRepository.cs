using PaymentGateway.Api.Domain.CardPayments;

namespace PaymentGateway.Api.Infrastructure.Repository
{
    public interface IPaymentsRepository
    {
        void AddOrUpdate(CardPayment payment);
        CardPayment? Get(Guid id);
    }
}