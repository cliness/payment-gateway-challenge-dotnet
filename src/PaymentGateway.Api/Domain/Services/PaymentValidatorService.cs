using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Infrastructure.Providers;

namespace PaymentGateway.Api.Domain.Services
{
    public class PaymentValidatorService : IPaymentValidatorService
    {
        private readonly IDateProvider _dateProvider;
        private readonly string[] _validCurrencyCodes;

        public PaymentValidatorService(IDateProvider dateProvider, string[] validCurrencyCodes)
        {
            _dateProvider = dateProvider;
            _validCurrencyCodes = validCurrencyCodes;
        }

        public bool IsNotValid(CardPayment payment)
        {
            var isCardPaymentValid = true;
            
            isCardPaymentValid &= payment.CardNumber.Length >= 14 && payment.CardNumber.Length <= 19 && payment.CardNumber.All(char.IsDigit);

            isCardPaymentValid &= payment.Cvv.Length >= 3 && payment.Cvv.Length <= 4 && payment.Cvv.All(char.IsDigit);

            isCardPaymentValid &= DateOnly.TryParseExact($"1/{payment.ExpiryMonth:00}/{payment.ExpiryYear}", "d/MM/yyyy", out var date) && date > _dateProvider.TodaysUtcDate();

            isCardPaymentValid &= payment.Amount > 0;

            isCardPaymentValid &= _validCurrencyCodes.Contains(payment.Currency);

            return !isCardPaymentValid;
        }
    }
}
