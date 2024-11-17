using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Repository;
using PaymentGateway.Api.Infrastructure.Translators;

namespace PaymentGateway.Api.Api.Controllers;

/// <summary>
/// Payment Controller for making and retrieving payments.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ICardPaymentService _cardPaymentService;

    /// <summary>
    /// Payment Controller for making and retrieving payments.
    /// </summary>
    /// <param name="paymentsRepository">Repository for storing and retrieving payments</param>
    /// <param name="cardPaymentService">Service for processing payments</param>
    public PaymentsController(IPaymentsRepository paymentsRepository, ICardPaymentService cardPaymentService)
    {
        _paymentsRepository = paymentsRepository;
        _cardPaymentService = cardPaymentService;
    }

    /// <summary>
    /// Makes a payment with provided payment details to the Acquiring Bank, so that funds can be transfered to the Merchant.
    /// </summary>
    /// <param name="paymentRequest">Captured payment details</param>
    /// <returns>Payment details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(ValidationProblemDetails))]
    public async Task<ActionResult<PostPaymentResponse>> MakePayment(PostPaymentRequest paymentRequest)
    {
        var cardPayment = paymentRequest.ToCardPayment(PaymentStatus.Requested, Guid.NewGuid());

        var payment = await _cardPaymentService.MakePayment(cardPayment);

        var paymentResponse = payment.ToPostPaymentResponse();
        return new OkObjectResult(paymentResponse);
    }

    /// <summary>
    /// Retrieves a payment using the ID returned from MakePayment post request.
    /// </summary>
    /// <param name="id">Payment Id</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    public ActionResult<GetPaymentResponse> GetPayment(Guid id)
    {
        var payment = _paymentsRepository.Get(id);
        if (payment == null)
        {
            return NotFound();
        }
        var paymentResponse = payment.ToGetPaymentResponse();
        return new OkObjectResult(paymentResponse);
    }
}