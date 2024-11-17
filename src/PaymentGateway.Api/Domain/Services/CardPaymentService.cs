
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
        private readonly IPaymentValidatorService _paymentValidatorService;

        public CardPaymentService(IAcquiringBankClient paymentGatewayClient, IPaymentsRepository paymentsRepository, IPaymentValidatorService paymentValidatorService)
        {
            _paymentGatewayClient = paymentGatewayClient;
            _paymentsRepository = paymentsRepository;
            _paymentValidatorService = paymentValidatorService;
        }

        public async Task<CardPayment> MakePayment(CardPayment payment)
        {
            _paymentsRepository.AddOrUpdate(payment);

            if (_paymentValidatorService.IsNotValid(payment))
            {
                payment.Status = PaymentStatus.Rejected;
                payment.AuthorizationCode = null;
                return payment;
            }

            var acquiringBankPaymentRequest = payment.ToAcquiringBankPaymentRequest();
            var acquiringBankPaymentResponse = await _paymentGatewayClient.PostPayment(acquiringBankPaymentRequest);

            if (acquiringBankPaymentResponse != null && acquiringBankPaymentResponse.Authorized)
            {
                payment.Status = PaymentStatus.Authorized;
                payment.AuthorizationCode = Guid.Parse(acquiringBankPaymentResponse.AuthorizationCode);
            }
            else
            {
                payment.Status = PaymentStatus.Declined;
                payment.AuthorizationCode = null;
            }

            _paymentsRepository.AddOrUpdate(payment);

            return payment;
        }
    }
}
