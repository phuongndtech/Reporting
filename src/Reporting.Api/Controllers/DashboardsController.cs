using Reporting.Application.Features.Dashboards.ReportingYear;
using Reporting.Domain.Entity.Charts;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("revenue-period")]
    [ProducesResponseType(typeof(RevenuePeriodChart), 200)]
    public async Task<IActionResult> GetRevenuePeriodChart()
        => Ok(await _mediator.Send(new Application.Features.Dashboards.Charts.RevenuePeriodChart.GetChart.Query()));

    [HttpGet("top-product")]
    [ProducesResponseType(typeof(TopProductRevenueChart), 200)]
    public async Task<IActionResult> GetTopProductRevenueChart()
        => Ok(await _mediator.Send(new Application.Features.Dashboards.Charts.TopProductRevenueChart.GetChart.Query()));

    [HttpGet("restaurant-revenue")]
    [ProducesResponseType(typeof(CompareRevenueChart), 200)]
    public async Task<IActionResult> GetCompareRevenueChart()
        => Ok(await _mediator.Send(new Application.Features.Dashboards.Charts.CompareRevenueChart.GetChart.Query()));

    [HttpGet("current-year")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> GetCurrentYearReporting()
        => Ok(await _mediator.Send(new GetYear.Query()));
}