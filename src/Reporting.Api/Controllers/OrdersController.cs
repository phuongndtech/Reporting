using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Features.Orders.Queries;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetList.Query
        {
            RestaurantType = Application.Common.Enums.RestaurantType.One
        });

        return Ok(result);
    }
}
