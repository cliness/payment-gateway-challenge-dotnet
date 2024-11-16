namespace PaymentGateway.Api.Models.AcquiringBank;

public class AcquiringBankPaymentResponse
{
    public required string AuthorizationCode { get; set; }
    public required bool Authorized { get; set; }
}
