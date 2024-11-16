using System.Net;

using PaymentGateway.Api.Models.AquiringBank;
using System.Text.Json;

namespace PaymentGateway.Api.Infrastructure
{
    public class AquiringBankClient : IAquiringBankClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _errorSerializeOptions;

        public AquiringBankClient(IHttpClientFactory httpClientFactory)
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

        public async Task<AquiringBankAuthorisation?> PostPayment(AquiringBankPaymentRequest paymentRequest)
        {
            using var _httpClient = _httpClientFactory.CreateClient(nameof(AquiringBankClient));
            var response = await _httpClient.PostAsJsonAsync("payments", paymentRequest, _serializeOptions);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paymentResponse = await response.Content.ReadFromJsonAsync<AquiringBankPaymentResponse>(_serializeOptions);

                Guid? authorisationCode = Guid.TryParse(paymentResponse?.AuthorizationCode, out var parsedAuthCode) ? parsedAuthCode : null;

                return new AquiringBankAuthorisation { AuthorizationCode = authorisationCode, Authorized = paymentResponse?.Authorized ?? false};
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AquiringPaymentErrorResponse>(_errorSerializeOptions);

                return new AquiringBankAuthorisation {AuthorizationCode = null, Authorized = false, ErrorMessage = errorResponse?.ErrorMessage };
            }
            return null;
        }
    }

    public class  AquiringBankAuthorisation
    {
        public required Guid? AuthorizationCode { get; set; }
        public required bool Authorized { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
