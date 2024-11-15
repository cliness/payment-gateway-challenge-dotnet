
using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Models.AquiringBank;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Translators;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Services
{
    public class CardPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly JsonSerializerOptions _serializeOptions;

        public CardPaymentService(IHttpClientFactory httpClientFactory, IPaymentsRepository paymentsRepository)
        {
            _httpClient = httpClientFactory.CreateClient("AquiringBankPayment");
            _paymentsRepository = paymentsRepository;

            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
        }

        public async Task MakePayment(CardPayment payment)
        {
            _paymentsRepository.AddOrUpdate(payment);

            var aquiringBankPayment = payment.ToAquiringBankPaymentRequest();

            var response = await _httpClient.PostAsJsonAsync("/payment", aquiringBankPayment, _serializeOptions);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var acquringBankPaymentResponse = await response.Content.ReadFromJsonAsync<AquiringBankPaymentResponse>(_serializeOptions);
                if (acquringBankPaymentResponse != null && acquringBankPaymentResponse.Authorized)
                {
                    payment.Status = Models.PaymentStatus.Authorized;
                    payment.AuthorizationCode = acquringBankPaymentResponse.AuthorizationCode;
                }
            }

            _paymentsRepository.AddOrUpdate(payment);
        }
    }
}
