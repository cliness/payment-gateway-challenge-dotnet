using System.Text.Json;

using PaymentGateway.Api.Infrastructure.Models;

namespace PaymentGateway.Api.Infrastructure.AcquiringBank
{
    public class AcquiringBankClient : IAcquiringBankClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializeOptions;

        public AcquiringBankClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
        }

        public async Task<AcquiringBankPaymentResponse> PostPayment(AcquiringBankPaymentRequest paymentRequest)
        {
            using var _httpClient = _httpClientFactory.CreateClient(nameof(AcquiringBankClient));
            var response = await _httpClient.PostAsJsonAsync("payments", paymentRequest, _serializeOptions);

            response.EnsureSuccessStatusCode();

            var paymentResponse = await response.Content.ReadFromJsonAsync<AcquiringBankPaymentResponse>(_serializeOptions);
            if (paymentResponse == null) {
                throw new Exception("Success status code but null response returned");
            }
            return paymentResponse;
        }
    }
}
