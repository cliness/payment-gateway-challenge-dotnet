namespace PaymentGateway.Api.Models.AquiringBank
{
    public class AquiringBankAuthorisation
    {
        public required Guid? AuthorizationCode { get; set; }
        public required bool Authorized { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
