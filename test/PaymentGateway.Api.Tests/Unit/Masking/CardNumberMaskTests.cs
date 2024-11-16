using PaymentGateway.Api.Masking;

namespace PaymentGateway.Api.Tests.Unit.Masking
{
    public class CardNumberMaskTests
    {       

        [Fact]
        public void ToLastFourDigits_CardNumber_ConvertsToLast4Digits()
        {
            //Arrange
            const long CardNumber = 2222405343248877;

            //Act
            var lastFourDigits = CardNumberMask.ToLastFourDigits(CardNumber);

            //Assert
            Assert.Equal(8877, lastFourDigits);
        }
    }
}
