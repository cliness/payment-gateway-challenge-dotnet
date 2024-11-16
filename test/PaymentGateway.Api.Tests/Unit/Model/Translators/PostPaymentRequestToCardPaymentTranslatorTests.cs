using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Models.Payments;
using PaymentGateway.Api.Models.Translators;

namespace PaymentGateway.Api.Tests.Unit.Model.Translators
{
    public class PostPaymentRequestToCardPaymentTranslatorTests
    {
        [Fact]
        public void ToCardPayment()
        {
            //Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Amount = 100,
                Currency = "GBP",
            };

            Guid id = Guid.NewGuid();
            PaymentStatus paymentStatus = PaymentStatus.Rejected;

            //Act
            var cardPayment = paymentRequest.ToCardPayment(paymentStatus, id);

            //Assert
            Assert.NotNull(cardPayment);
            Assert.Equal(id, cardPayment.Id);
            Assert.Equal(2222405343248877, cardPayment.CardNumber);
            Assert.Equal("123", cardPayment.Cvv);
            Assert.Equal(4, cardPayment.ExpiryMonth);
            Assert.Equal(2025, cardPayment.ExpiryYear);
            Assert.Equal(100, cardPayment.Amount);
            Assert.Equal("GBP", cardPayment.Currency);
            Assert.Equal(PaymentStatus.Rejected, cardPayment.Status);
        }
    }
}
