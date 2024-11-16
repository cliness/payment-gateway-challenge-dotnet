namespace PaymentGateway.Api.Masking
{
    public static class CardNumberMask
    {
        public static int ToLastFourDigits(this long cardNumber)
        {
            var cardNumberAsString = cardNumber.ToString();
            var lastFourDigitsAsString = cardNumberAsString.Substring(cardNumberAsString.Length - 4, 4);
            return int.Parse(lastFourDigitsAsString);
        }
    }
}
