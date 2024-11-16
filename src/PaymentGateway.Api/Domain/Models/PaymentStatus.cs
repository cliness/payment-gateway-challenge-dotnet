namespace PaymentGateway.Api.Domain.Models;

public enum PaymentStatus
{
    Requested,
    Authorized,
    Declined,
    Rejected
}