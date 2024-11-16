using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.CardPayments;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Tests.Unit.Repository
{
    public class InMemoryPaymentsRepositoryTests
    {
        [Fact]
        public void AddOrUpdate_AddsNewPayment_RetrievesSamePayment()
        {
            //Arrange
            Guid cardPaymentId = Guid.NewGuid();
            var cardPayment = new CardPayment()
            {
                Id = cardPaymentId,
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Status = PaymentStatus.Requested,
            };

            var inMemoryPaymentRepository = new InMemoryPaymentsRepository();

            //Act
            inMemoryPaymentRepository.AddOrUpdate(cardPayment);
            var foundCardPayment = inMemoryPaymentRepository.Get(cardPaymentId);

            //Assert
            Assert.NotNull(foundCardPayment);
            Assert.Equal(cardPaymentId, foundCardPayment.Id);
            Assert.Equal(2222405343248877, foundCardPayment.CardNumber);
            Assert.Equal(2025, foundCardPayment.ExpiryYear);
            Assert.Equal(4, foundCardPayment.ExpiryMonth);

            Assert.Equal(100, foundCardPayment.Amount);
            Assert.Equal("GBP", foundCardPayment.Currency);

            Assert.Equal(PaymentStatus.Requested, foundCardPayment.Status);
        }
        
        [Fact]
        public void AddOrUpdate_UpdatesPayment_RetrievesSamePayment()
        {
            //Arrange
            Guid cardPaymentId = Guid.NewGuid();
            var firstCardPayment = new CardPayment
            {
                Id = cardPaymentId,
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Status = PaymentStatus.Requested,
            };
            var secondCardPayment = new CardPayment
            {
                Id = cardPaymentId,
                CardNumber = 2222405343248877,
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Status = PaymentStatus.Authorized,
            };

            var inMemoryPaymentRepository = new InMemoryPaymentsRepository();

            //Act
            inMemoryPaymentRepository.AddOrUpdate(firstCardPayment);
            inMemoryPaymentRepository.AddOrUpdate(secondCardPayment);
            var foundCardPayment = inMemoryPaymentRepository.Get(cardPaymentId);

            //Assert
            Assert.Equal(cardPaymentId, foundCardPayment?.Id);
            Assert.Equal(PaymentStatus.Authorized, foundCardPayment?.Status);
        }        
        
        [Fact]
        public void Get_NoPaymentsStored_RetrievesNoPayment()
        {
            //Arrange
            Guid cardPaymentId = Guid.NewGuid();

            var inMemoryPaymentRepository = new InMemoryPaymentsRepository();

            //Act
            var foundCardPayment = inMemoryPaymentRepository.Get(cardPaymentId);

            //Assert
            Assert.Null(foundCardPayment);
        }
    }
}
