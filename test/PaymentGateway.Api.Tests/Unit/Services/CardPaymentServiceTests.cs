using Moq;

using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.AcquiringBank;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Unit.Services
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
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Status = PaymentStatus.Requested,
            };
            InMemoryPaymentsRepository paymentsRepository = new InMemoryPaymentsRepository();

            var bankPaymentResponse = new AcquiringBankAuthorisation { AuthorizationCode = Guid.NewGuid(), Authorized = true };
            var paymentGatewayClient = new Mock<IAcquiringBankClient>();
            paymentGatewayClient.Setup(client => client.PostPayment(It.IsAny<AcquiringBankPaymentRequest>())).ReturnsAsync(bankPaymentResponse);

            var cardPaymentService = new CardPaymentService(paymentGatewayClient.Object, paymentsRepository);

            //Act
            var returnedCardPayment = await cardPaymentService.MakePayment(cardPayment);

            //Assert
            Assert.NotNull(returnedCardPayment);
            Assert.Equal(PaymentStatus.Authorized, returnedCardPayment.Status);
            Assert.NotNull(returnedCardPayment.AuthorizationCode);

            Assert.Equal(cardPayment.Id, returnedCardPayment.Id);
            Assert.Equal(2222405343248877, returnedCardPayment.CardNumber);
            Assert.Equal(2025, returnedCardPayment.ExpiryYear);
            Assert.Equal(4, returnedCardPayment.ExpiryMonth);

            Assert.Equal(100, returnedCardPayment.Amount);
            Assert.Equal("GBP", returnedCardPayment.Currency);
        }
    }
}
