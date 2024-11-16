using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Services
{
    public class CardPaymentService : ICardPaymentService
    {
        private readonly IAcquiringBankClient _paymentGatewayClient;
        private readonly IPaymentsRepository _paymentsRepository;

        public CardPaymentService(IAcquiringBankClient paymentGatewayClient, IPaymentsRepository paymentsRepository)
        {
            _paymentGatewayClient = paymentGatewayClient;
            _paymentsRepository = paymentsRepository;
        }

        public async Task<CardPayment> MakePayment(CardPayment payment)
        {
            _paymentsRepository.AddOrUpdate(payment);

            var acquiringBankPaymentRequest = payment.ToAcquiringBankPaymentRequest();

            var acquiringBankPaymentResponse = await _paymentGatewayClient.PostPayment(acquiringBankPaymentRequest);

            if (acquiringBankPaymentResponse != null && acquiringBankPaymentResponse.Authorized)
            {
                payment.Status = Models.PaymentStatus.Authorized;
                payment.AuthorizationCode = acquiringBankPaymentResponse.AuthorizationCode;
            }
            else
            {
                throw new NotImplementedException();
            }            

            _paymentsRepository.AddOrUpdate(payment);

            return payment;
        }
    }
}
