using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Models;
using Reporting.Application.Features.ProductPrices.Queries;

namespace Reporting.Api.Controllers;

[Route("api/product-prices")]
[ApiController]
public class ProductPricesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] RequestBase request)
    {
        var result = await _mediator.Send(new GetList.Query
        {
            Type = request.Type,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });

        return Ok(result);
    }
}
