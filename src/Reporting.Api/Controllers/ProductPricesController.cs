using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Enums;
using Reporting.Application.Features.ProductPrices.Queries;

namespace Reporting.Api.Controllers;

[Route("api/product-prices")]
[ApiController]
public class ProductPricesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] RestaurantType type)
    {
        var result = await _mediator.Send(new GetList.Query
        {
            RestaurantType = type
        });

        return Ok(result);
    }
}
