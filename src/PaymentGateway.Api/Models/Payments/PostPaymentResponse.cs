﻿namespace PaymentGateway.Api.Models.Payments;

public class PostPaymentResponse
{
    public required Guid Id { get; set; }
    public required PaymentStatus Status { get; set; }
    public required int CardNumberLastFour { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required int Amount { get; set; }
    public required string Currency { get; set; }    
}
