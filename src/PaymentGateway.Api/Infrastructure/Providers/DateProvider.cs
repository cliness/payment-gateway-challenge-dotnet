namespace PaymentGateway.Api.Infrastructure.Providers
{
    public class DateProvider : IDateProvider
    {
        public DateOnly TodaysUtcDate()
        {
            return DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}
