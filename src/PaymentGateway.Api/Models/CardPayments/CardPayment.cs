namespace PaymentGateway.Api.Models.CardPayments
{
    public class CardPayment
    {
        public Guid Id { get; set; }
        public long CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? FailureReason { get; set; }
    }
}
