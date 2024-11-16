using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Models.AcquiringBank;

namespace PaymentGateway.Api.Infrastructure
{
    public class AcquiringBankClient : IAcquiringBankClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _errorSerializeOptions;

        public AcquiringBankClient(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
            
            _errorSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<AcquiringBankAuthorisation?> PostPayment(AcquiringBankPaymentRequest paymentRequest)
        {
            using var _httpClient = _httpClientFactory.CreateClient(nameof(AcquiringBankClient));
            var response = await _httpClient.PostAsJsonAsync("payments", paymentRequest, _serializeOptions);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paymentResponse = await response.Content.ReadFromJsonAsync<AcquiringBankPaymentResponse>(_serializeOptions);

                Guid? authorisationCode = Guid.TryParse(paymentResponse?.AuthorizationCode, out var parsedAuthCode) ? parsedAuthCode : null;

                return new AcquiringBankAuthorisation { AuthorizationCode = authorisationCode, Authorized = paymentResponse?.Authorized ?? false};
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AcquiringPaymentErrorResponse>(_errorSerializeOptions);

                return new AcquiringBankAuthorisation {AuthorizationCode = null, Authorized = false, ErrorMessage = errorResponse?.ErrorMessage };
            }
            return null;
        }
    }
}
