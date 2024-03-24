using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Models;
using Reporting.Application.Features.Orders.Queries;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] RequestBase request)
    {
        var result = await _mediator.Send(new GetList.Query
        {
            Type = request.Type
        });

        return Ok(result);
    }
}
