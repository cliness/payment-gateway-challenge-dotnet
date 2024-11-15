namespace PaymentGateway.Api.Tests.Configuration
{
    public class AquiringPaymentSettings
    {
        public const string Section = "AquiringPayment";

        public Uri? ServiceEndpoint { get; set; }
    }
}

