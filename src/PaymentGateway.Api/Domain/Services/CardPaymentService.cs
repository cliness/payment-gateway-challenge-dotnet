using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Infrastructure.AcquiringBank;
using PaymentGateway.Api.Infrastructure.Repository;
using PaymentGateway.Api.Infrastructure.Translators;

namespace PaymentGateway.Api.Domain.Services
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
                payment.Status = PaymentStatus.Authorized;
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
