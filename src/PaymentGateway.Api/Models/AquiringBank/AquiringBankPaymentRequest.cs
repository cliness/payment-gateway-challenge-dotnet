namespace PaymentGateway.Api.Models.AquiringBank;

public class AquiringBankPaymentRequest
{
    public required long CardNumber { get; set; }
    public required string ExpiryDate { get; set; }
    public required string Currency { get; set; }
    public required int Amount { get; set; }
    public required string Cvv { get; set; }
}
