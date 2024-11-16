using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Models.AquiringBank;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Services
{
    public class CardPaymentService : ICardPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly JsonSerializerOptions _serializeOptions;

        public CardPaymentService(IHttpClientFactory httpClientFactory, IPaymentsRepository paymentsRepository)
        {
            _httpClientFactory = httpClientFactory;
            _paymentsRepository = paymentsRepository;

            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
        }

        public async Task<CardPayment> MakePayment(CardPayment payment)
        {
            _paymentsRepository.AddOrUpdate(payment);

            var aquiringBankPaymentRequest = payment.ToAquiringBankPaymentRequest();

            using var _httpClient = _httpClientFactory.CreateClient(nameof(CardPaymentService));
            var response = await _httpClient.PostAsJsonAsync("payments", aquiringBankPaymentRequest, _serializeOptions);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var acquringBankPaymentResponse = await response.Content.ReadFromJsonAsync<AquiringBankPaymentResponse>(_serializeOptions);
                if (acquringBankPaymentResponse != null && acquringBankPaymentResponse.Authorized)
                {
                    payment.Status = Models.PaymentStatus.Authorized;
                    payment.AuthorizationCode = acquringBankPaymentResponse.AuthorizationCode;
                }
                else
                {
                    throw new NotImplementedException();
                }
            } else
            {
                throw new NotImplementedException();
            }

            _paymentsRepository.AddOrUpdate(payment);

            return payment;
        }
    }
}
