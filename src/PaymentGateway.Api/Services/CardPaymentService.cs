using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Models.AquiringBank;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Services
{
    public class CardPaymentService : ICardPaymentService
    {
        private readonly IAquiringBankClient _paymentGatewayClient;
        private readonly IPaymentsRepository _paymentsRepository;

        public CardPaymentService(IAquiringBankClient paymentGatewayClient, IPaymentsRepository paymentsRepository)
        {
            _paymentGatewayClient = paymentGatewayClient;
            _paymentsRepository = paymentsRepository;
        }

        public async Task<CardPayment> MakePayment(CardPayment payment)
        {
            _paymentsRepository.AddOrUpdate(payment);

            var aquiringBankPaymentRequest = payment.ToAquiringBankPaymentRequest();

            var aquiringBankPaymentResponse = await _paymentGatewayClient.PostPayment(aquiringBankPaymentRequest);

            if (aquiringBankPaymentResponse != null && aquiringBankPaymentResponse.Authorized)
            {
                payment.Status = Models.PaymentStatus.Authorized;
                payment.AuthorizationCode = aquiringBankPaymentResponse.AuthorizationCode;
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
