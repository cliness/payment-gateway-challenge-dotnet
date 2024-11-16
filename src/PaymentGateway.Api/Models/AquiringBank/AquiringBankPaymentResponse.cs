using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.AquiringBank;

public class AquiringBankPaymentResponse
{
    public required string AuthorizationCode { get; set; }
    public required bool Authorized { get; set; }
}
