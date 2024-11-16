using PaymentGateway.Api.Models.AquiringBank;

namespace PaymentGateway.Api.Infrastructure
{
    public interface IAquiringBankClient
    {
        Task<AquiringBankAuthorisation?> PostPayment(AquiringBankPaymentRequest paymentRequest);
    }
}