using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Interfaces;
using Reporting.Application.Common.Models;
using Reporting.Application.Features.Orders.Queries;
using System.Net.Mime;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator, IExcelReader excelReader) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    private readonly IExcelReader _excelReader = excelReader;

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

        return Ok(result.Orders.OrderBy(x=>x.OrderNumber));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var result = await _mediator.Send(new Export.Query());

        result.FileStream.Position = 0;

         return File(result.FileStream, MediaTypeNames.Application.Octet, "restaurant_data.xlsx");
    }
}