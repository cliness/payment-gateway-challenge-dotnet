﻿using PaymentGateway.Api.Domain.CardPayments;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Infrastructure.Translators;

namespace PaymentGateway.Api.Tests.Unit.Infrastructure.Translators
{
    public class PaymentToAcquiringBankPaymentRequestTranslatorTests
    {
        [Fact]
        public void ToAcquiringBankPaymentRequest_CardPayment_TranslatesToRequest()
        {
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid(),

                CardNumber = "2222405343248112",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2026,

                Amount = 100,
                Currency = "GBP",

                AuthorizationCode = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
            };

            var acquiringBankPaymentRequest = cardPayment.ToAcquiringBankPaymentRequest();

            Assert.Equal("2222405343248112", acquiringBankPaymentRequest.CardNumber);
            Assert.Equal("04/2026", acquiringBankPaymentRequest.ExpiryDate);
            Assert.Equal("123", acquiringBankPaymentRequest.Cvv);
            Assert.Equal(100, acquiringBankPaymentRequest.Amount);
            Assert.Equal("GBP", acquiringBankPaymentRequest.Currency);
        }
    }
}
