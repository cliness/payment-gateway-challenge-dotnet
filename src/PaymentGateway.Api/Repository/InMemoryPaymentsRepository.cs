using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Repository;

public class InMemoryPaymentsRepository: IPaymentsRepository
{
    private readonly Dictionary<Guid, CardPayment> _payments = [];

    public void AddOrUpdate(CardPayment payment)
    {
        if (_payments.TryGetValue(payment.Id, out _))
        {
            _payments[payment.Id] = payment;
        }
        else
        {
            _payments.Add(payment.Id, payment);            
        }
    }

    public CardPayment? Get(Guid id)
    {
        return _payments.TryGetValue(id, out var cardPayment) ? cardPayment : null;
    }
}