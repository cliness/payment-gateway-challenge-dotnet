using PaymentGateway.Api.Infrastructure.Models;

namespace PaymentGateway.Api.Infrastructure.AcquiringBank
{
    public interface IAcquiringBankClient
    {
        Task<AcquiringBankAuthorisation?> PostPayment(AcquiringBankPaymentRequest paymentRequest);
    }
}