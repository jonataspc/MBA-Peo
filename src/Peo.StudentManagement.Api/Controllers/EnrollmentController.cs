using MediatR;
using Microsoft.AspNetCore.Mvc;
using Peo.StudentManagement.Application.Commands.ProcessEnrollmentPayment;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("payment")]
    [ProducesResponseType(typeof(ProcessEnrollmentPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProcessEnrollmentPaymentResponse>> ProcessPayment([FromBody] ProcessEnrollmentPaymentRequest request)
    {
        var command = new ProcessEnrollmentPaymentCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }
} 