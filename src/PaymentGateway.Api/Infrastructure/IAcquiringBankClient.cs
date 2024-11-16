using PaymentGateway.Api.Models.AcquiringBank;

namespace PaymentGateway.Api.Infrastructure
{
    public interface IAcquiringBankClient
    {
        Task<AcquiringBankAuthorisation?> PostPayment(AcquiringBankPaymentRequest paymentRequest);
    }
}