namespace PaymentGateway.Api.Models.Payments;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; }    
}
