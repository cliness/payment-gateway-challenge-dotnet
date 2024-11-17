namespace PaymentGateway.Api.Infrastructure.Models;

public class AcquiringBankPaymentRequest
{
    public required string CardNumber { get; set; }
    public required string ExpiryDate { get; set; }
    public required string Currency { get; set; }
    public required int Amount { get; set; }
    public required string Cvv { get; set; }
}
