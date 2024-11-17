using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Api.Models;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Repository;
using PaymentGateway.Api.Infrastructure.Translators;

namespace PaymentGateway.Api.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ICardPaymentService _cardPaymentService;

    public PaymentsController(IPaymentsRepository paymentsRepository, ICardPaymentService cardPaymentService)
    {
        _paymentsRepository = paymentsRepository;
        _cardPaymentService = cardPaymentService;
    }

    [HttpPost()]
    public async Task<ActionResult<PostPaymentResponse>> MakePayment(PostPaymentRequest paymentRequest)
    {
        var cardPayment = paymentRequest.ToCardPayment(PaymentStatus.Requested, Guid.NewGuid());

        var payment = await _cardPaymentService.MakePayment(cardPayment);

        var paymentResponse = payment.ToPostPaymentResponse();
        return new OkObjectResult(paymentResponse);
    }

    [HttpGet("{id:guid}")]
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