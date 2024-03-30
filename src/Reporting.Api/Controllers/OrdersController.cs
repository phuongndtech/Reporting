using Reporting.Application.Features.Orders.Queries;
using System.Net.Mime;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private const string FileName = "restaurant_data.xlsx";

    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(GetList.Result), 200)]
    public async Task<IActionResult> Get([FromQuery] OrderRequest request)
    {
        var result = await _mediator.Send(new GetList.Query
        {
            Type = request.Type,
            OrderNumber = request.OrderNumber,
            SearchText = request.SearchText
        });

        return Ok(result.Orders.OrderBy(x => x.OrderNumber));
    }

    [HttpGet("export")]
    [ProducesResponseType(typeof(Export.Result), 200)]
    public async Task<IActionResult> Export()
    {
        var result = await _mediator.Send(new Export.Query());

        result.FileStream.Position = 0;

        return File(result.FileStream, MediaTypeNames.Application.Octet, FileName);
    }
}