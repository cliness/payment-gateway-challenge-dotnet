namespace PaymentGateway.Api.Infrastructure.Providers
{
    public interface IDateProvider
    {
        DateOnly TodaysUtcDate();
    }
}
