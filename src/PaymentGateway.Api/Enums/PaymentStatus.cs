namespace PaymentGateway.Api.Models;

public enum PaymentStatus
{
    Requested,
    Authorized,
    Declined,
    Rejected
}