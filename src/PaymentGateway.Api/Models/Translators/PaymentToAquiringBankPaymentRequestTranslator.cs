﻿using PaymentGateway.Api.Models.AquiringBank;
using PaymentGateway.Api.Models.CardPayments;

namespace PaymentGateway.Api.Models.Translators
{
    public static class PaymentToAquiringBankPaymentRequestTranslator
    {
        public static AquiringBankPaymentRequest ToAquiringBankPaymentRequest(this CardPayment payment)
        {
            return new AquiringBankPaymentRequest
            {
                CardNumber = payment.CardNumber,
                Cvv = payment.Cvv,
                ExpiryDate = $"{payment.ExpiryMonth:00}/{payment.ExpiryYear}",
                Amount = payment.Amount,
                Currency = payment.Currency,
            };
        }
    }
}