using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Payments;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly InMemoryPaymentsRepository _paymentsRepository;

    public PaymentsController(InMemoryPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpPost()]
    public ActionResult<PostPaymentResponse> MakePayment(PostPaymentRequest paymentRequest) {
        return new OkObjectResult(paymentRequest);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GetPaymentResponse> GetPayment(Guid id)
    {
        var payment = _paymentsRepository.Get(id);
        if(payment == null)
        {
            return NotFound();
        }
        return new OkObjectResult(payment);
    }
}