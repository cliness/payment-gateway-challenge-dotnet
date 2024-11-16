﻿namespace PaymentGateway.Api.Models.CardPayments
{
    public class CardPayment
    {
        public required Guid Id { get; set; }
        public required long CardNumber { get; set; }
        public required int ExpiryMonth { get; set; }
        public required int ExpiryYear { get; set; }
        public required string Cvv { get; set; }
        public required string Currency { get; set; }
        public required int Amount { get; set; }
        public required PaymentStatus Status { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? FailureReason { get; set; }
    }
}