namespace PaymentGateway.Api.Infrastructure.Configuration
{    
    // The record type causes the ValidCurrencyCodes to have duplicate entries. [ChrisL, 17/11/2024]
    public class PaymentGatewaySettings
    {
        public required string[] ValidCurrencyCodes { get; set; }
    };
}

