namespace PaymentGateway.Api.Models.Requests;

public class AquiringBankPaymentRequest
{
    public long CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string Cvv { get; set; }
}
