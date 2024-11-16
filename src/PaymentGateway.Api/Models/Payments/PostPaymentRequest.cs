namespace PaymentGateway.Api.Models.Payments;

public class PostPaymentRequest
{
    public long CardNumber { get; set; }
    public string Cvv { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; }   
    
}