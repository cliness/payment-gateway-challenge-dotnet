namespace PaymentGateway.Api.Models.Responses;

public class AquiringBankPaymentResponse
{
    public string AuthorizationCode { get; set; }
    public bool Authorized { get; set; }
}
