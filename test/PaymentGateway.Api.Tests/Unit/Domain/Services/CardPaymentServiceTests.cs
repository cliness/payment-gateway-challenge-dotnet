using Moq;

using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.AcquiringBank;
using PaymentGateway.Api.Infrastructure.Models;
using PaymentGateway.Api.Infrastructure.Repository;

namespace PaymentGateway.Api.Tests.Unit.Domain.Services
{
    public class CardPaymentServiceTests
    {
        [Fact]
        public async Task MakePayment_AuthorisedCard_ReturnsAuthorisedPayment()
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "2222405343248877",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Status = PaymentStatus.Requested,
            };
            InMemoryPaymentsRepository paymentsRepository = new InMemoryPaymentsRepository();

            var bankPaymentResponse = new AcquiringBankAuthorisation { AuthorizationCode = Guid.NewGuid(), Authorized = true };
            var paymentGatewayClientMock = new Mock<IAcquiringBankClient>();
            paymentGatewayClientMock.Setup(client => client.PostPayment(It.IsAny<AcquiringBankPaymentRequest>())).ReturnsAsync(bankPaymentResponse);

            Mock<IPaymentValidatorService> paymentValidatorServiceMock = new Mock<IPaymentValidatorService>();
            paymentValidatorServiceMock.Setup(service => service.IsNotValid(It.IsAny<CardPayment>())).Returns(false);

            var cardPaymentService = new CardPaymentService(paymentGatewayClientMock.Object, paymentsRepository, paymentValidatorServiceMock.Object);

            //Act
            var returnedCardPayment = await cardPaymentService.MakePayment(cardPayment);

            //Assert
            Assert.NotNull(returnedCardPayment);
            Assert.Equal(PaymentStatus.Authorized, returnedCardPayment.Status);
            Assert.NotNull(returnedCardPayment.AuthorizationCode);

            Assert.Equal(cardPayment.Id, returnedCardPayment.Id);
            Assert.Equal("2222405343248877", returnedCardPayment.CardNumber);
            Assert.Equal("123", returnedCardPayment.Cvv);
            Assert.Equal(2025, returnedCardPayment.ExpiryYear);
            Assert.Equal(4, returnedCardPayment.ExpiryMonth);

            Assert.Equal(100, returnedCardPayment.Amount);
            Assert.Equal("GBP", returnedCardPayment.Currency);
        }        
        
        [Fact]
        public async Task MakePayment_NotAuthorisedCard_ReturnsDeclinedPayment()
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
            InMemoryPaymentsRepository paymentsRepository = new InMemoryPaymentsRepository();

            var bankPaymentResponse = new AcquiringBankAuthorisation { AuthorizationCode = null, Authorized = false };
            var paymentGatewayClientMock = new Mock<IAcquiringBankClient>();
            paymentGatewayClientMock.Setup(client => client.PostPayment(It.IsAny<AcquiringBankPaymentRequest>())).ReturnsAsync(bankPaymentResponse);

            Mock<IPaymentValidatorService> paymentValidatorServiceMock = new Mock<IPaymentValidatorService>();
            paymentValidatorServiceMock.Setup(service => service.IsNotValid(It.IsAny<CardPayment>())).Returns(false);

            var cardPaymentService = new CardPaymentService(paymentGatewayClientMock.Object, paymentsRepository, paymentValidatorServiceMock.Object);

            //Act
            var returnedCardPayment = await cardPaymentService.MakePayment(cardPayment);

            //Assert
            Assert.NotNull(returnedCardPayment);
            Assert.Equal(PaymentStatus.Declined, returnedCardPayment.Status);
            Assert.Null(returnedCardPayment.AuthorizationCode);

            Assert.Equal(cardPayment.Id, returnedCardPayment.Id);
            Assert.Equal("2222405343248112", returnedCardPayment.CardNumber);
            Assert.Equal("456", returnedCardPayment.Cvv);
            Assert.Equal(2026, returnedCardPayment.ExpiryYear);
            Assert.Equal(1, returnedCardPayment.ExpiryMonth);

            Assert.Equal(60000, returnedCardPayment.Amount);
            Assert.Equal("USD", returnedCardPayment.Currency);
        }
        
        [Fact]
        public async Task MakePayment_InvalidCard_ReturnsRejectedPayment()
        {
            //Arrange
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),
                CardNumber = "11111111111111111111",
                Cvv = "99999",
                ExpiryMonth = 1,
                ExpiryYear = 2013,
                Currency = "USD",
                Amount = 60000,
                Status = PaymentStatus.Requested,
            };

            InMemoryPaymentsRepository paymentsRepository = new InMemoryPaymentsRepository();

            var paymentGatewayClientMock = new Mock<IAcquiringBankClient>();

            Mock<IPaymentValidatorService> paymentValidatorServiceMock = new Mock<IPaymentValidatorService>();
            paymentValidatorServiceMock.Setup(service => service.IsNotValid(It.IsAny<CardPayment>())).Returns(true);

            var cardPaymentService = new CardPaymentService(paymentGatewayClientMock.Object, paymentsRepository, paymentValidatorServiceMock.Object);

            //Act
            var returnedCardPayment = await cardPaymentService.MakePayment(cardPayment);

            //Assert
            Assert.NotNull(returnedCardPayment);
            Assert.Equal(PaymentStatus.Rejected, returnedCardPayment.Status);
            Assert.Null(returnedCardPayment.AuthorizationCode);
            Assert.Null(returnedCardPayment.FailureReason);

            Assert.Equal(cardPayment.Id, returnedCardPayment.Id);
            Assert.Equal("11111111111111111111", returnedCardPayment.CardNumber);
            Assert.Equal("99999", returnedCardPayment.Cvv);
            Assert.Equal(2013, returnedCardPayment.ExpiryYear);
            Assert.Equal(1, returnedCardPayment.ExpiryMonth);            

            Assert.Equal(60000, returnedCardPayment.Amount);
            Assert.Equal("USD", returnedCardPayment.Currency);
        }
    }
}
