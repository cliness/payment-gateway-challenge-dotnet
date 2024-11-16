namespace PaymentGateway.Api.Models.AcquiringBank
{
    public class AcquiringBankAuthorisation
    {
        public required Guid? AuthorizationCode { get; set; }
        public required bool Authorized { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
