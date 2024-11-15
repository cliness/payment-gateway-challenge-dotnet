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
            var cardPayment = new CardPayment() { Id = cardPaymentId, Amount = 1000 };

            var inMemoryPaymentRepository = new InMemoryPaymentsRepository();

            //Act
            inMemoryPaymentRepository.AddOrUpdate(cardPayment);
            var foundCardPayment = inMemoryPaymentRepository.Get(cardPaymentId);

            //Assert
            Assert.Equal(cardPaymentId, foundCardPayment?.Id);
            Assert.Equal(1000, foundCardPayment?.Amount);
        }
        
        [Fact]
        public void AddOrUpdate_UpdatesPayment_RetrievesSamePayment()
        {
            //Arrange
            Guid cardPaymentId = Guid.NewGuid();
            var firstCardPayment = new CardPayment() { Id = cardPaymentId, Amount = 1000 };
            var secondCardPayment = new CardPayment() { Id = cardPaymentId, Amount = 1123 };

            var inMemoryPaymentRepository = new InMemoryPaymentsRepository();

            //Act
            inMemoryPaymentRepository.AddOrUpdate(firstCardPayment);
            inMemoryPaymentRepository.AddOrUpdate(secondCardPayment);
            var foundCardPayment = inMemoryPaymentRepository.Get(cardPaymentId);

            //Assert
            Assert.Equal(cardPaymentId, foundCardPayment?.Id);
            Assert.Equal(1123, foundCardPayment?.Amount);
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
