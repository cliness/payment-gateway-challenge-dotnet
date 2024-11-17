# Payment Gateway API

The Payment Gateway API enables payments to be made and retrieved.

## Solution

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Folder structure

```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

## Developer instructions

Please remember to start the imposter first using `.\docker-compose up` and it listens on `http://localhost:8080/`.
Then start the project and/or run the integration tests.
An OpenAPI endpoint is available for testing the endpoints. 

Example authorised payment request for `/api/Payments` is:
```
{
  "cardNumber": "2222405343248877",
  "cvv": "123",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "amount": 100,
  "currency": "GBP"
}
```
Example declined payment request for `/api/Payments` is:
```
{
  "cardNumber": "2222405343248112",
  "cvv": "456",
  "expiryMonth": 1,
  "expiryYear": 2026,
  "amount": 60000,
  "currency": "USD"
}
```
Example rejected payment request for `/api/Payments` is:
```
{
  "cardNumber": "A222240533248112",
  "cvv": "456",
  "expiryMonth": 1,
  "expiryYear": 2026,
  "amount": 60000,
  "currency": "USD"
}
```
