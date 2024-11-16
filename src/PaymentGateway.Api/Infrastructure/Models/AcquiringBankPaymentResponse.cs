namespace PaymentGateway.Api.Infrastructure.Models;

public class AcquiringBankPaymentResponse
{
    public required string AuthorizationCode { get; set; }
    public required bool Authorized { get; set; }
}
