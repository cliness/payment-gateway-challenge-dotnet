namespace PaymentGateway.Api.Api.Models;

public class PostPaymentRequest
{
    public required long CardNumber { get; set; }
    public required string Cvv { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required int Amount { get; set; }
    public required string Currency { get; set; }

}