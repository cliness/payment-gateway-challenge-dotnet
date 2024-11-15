namespace PaymentGateway.Api.Models.AquiringBank;

public class AquiringBankPaymentResponse
{
    public string AuthorizationCode { get; set; }
    public bool Authorized { get; set; }
}
