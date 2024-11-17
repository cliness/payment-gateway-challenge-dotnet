using Moq;

using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Providers;

namespace PaymentGateway.Api.Tests.Unit.Domain.Services
{
    public class PaymentValidatorServiceTests
    {
        [Fact]
        public void IsNotValid_ValidPayment_ReturnsFalse()
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = "456",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = 60000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.False(result);
        }
        
        [Theory]
        [InlineData("2222405343248A112")]
        [InlineData("22224053432481211212")]
        [InlineData("2222405343248")]
        public void IsNotValid_CardNumberInvalid_ReturnsTrue(string cardNumber)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = cardNumber,
                Cvv = "456",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = 60000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.True(result);
        }
        
        [Theory]
        [InlineData("12345")]
        [InlineData("12")]
        [InlineData("12a")]
        public void IsNotValid_CvvInvalid_ReturnsTrue(string cvv)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = cvv,
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = 60000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.True(result);
        }
        
        [Theory]
        [InlineData(13, 2026)]
        [InlineData(11, 2024)]
        [InlineData(12, -1)]
        [InlineData(-1, 2026)]
        public void IsNotValid_ExpiryInvalid_ReturnsTrue(int month, int year)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = "456",
                ExpiryMonth = month,
                ExpiryYear = year,
                Currency = "USD",
                Amount = 60000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsNotValid_InvalidAmmount_ReturnsTrue(int amount)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = "456",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = amount,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.True(result);
        }
        
        [Theory]
        [InlineData("USD")]
        [InlineData("GBP")]
        [InlineData("EUR")]
        public void IsNotValid_ValidCurrencyCode_ReturnsFalse(string currencyCode)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = "456",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = currencyCode,
                Amount = 6000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.False(result);
        }
        
        [Theory]
        [InlineData("JPY")]
        [InlineData("CHF")]
        [InlineData("AUD")]
        [InlineData("CAD")]
        [InlineData("CNY")]
        [InlineData("NZD")]
        [InlineData("INR")]
        [InlineData("BZR")]
        [InlineData("SEK")]
        [InlineData("ZAR")]
        [InlineData("HKD")]
        public void IsNotValid_OtherMajorCurrencyCode_ReturnsTrue(string currencyCode)
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248112",
                Cvv = "456",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = currencyCode,
                Amount = 6000,
                Status = PaymentStatus.Requested,
            };

            var dateProviderMock = new Mock<IDateProvider>();
            dateProviderMock.Setup(provider => provider.TodaysUtcDate()).Returns(new DateOnly(2024, 11, 17));

            var paymentValidatorService = new PaymentValidatorService(dateProviderMock.Object);

            //Act
            var result = paymentValidatorService.IsNotValid(cardPayment);

            //Assert
            Assert.True(result);
        }
    }
}
