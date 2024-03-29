using MediatR;
using Reporting.Application.Common.Interfaces;
using Reporting.Application.Common.Models;
using Reporting.Domain.Entity;
using static Reporting.Application.Features.Orders.Queries.GetList;

namespace Reporting.Application.Features.Orders.Queries;

public class GetList
{
    public class Query : RequestBase, IRequest<Result>
    {
        public decimal? ProductPrice { get; set; }
        public int? OrderNumber { get; set; }
    }

    public class Result
    {
        public List<Order> Orders { get; set; }
        public int TotalCount { get; set; }
    }
}

public class Handler(IExcelReader excelReader) : IRequestHandler<Query, Result>
{
    private readonly IExcelReader _excelReader = excelReader;

    public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
    {
        var orders = await _excelReader.GetOrders(request.Type);

        var dataFilter = orders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchText) && !request.SearchText.Equals("undefined"))
        {
            var trimData = request.SearchText.Trim();

            dataFilter = dataFilter
                .Where(x => x.ItemName.Equals(trimData, StringComparison.OrdinalIgnoreCase));
        }

        if (request.ProductPrice.HasValue)
        {
            dataFilter = dataFilter
                .Where(x => x.ProductPrice == request.ProductPrice);
        }

        if (request.OrderNumber.HasValue)
        {
            dataFilter = dataFilter
                .Where(x => x.OrderNumber == request.OrderNumber);
        }

        var result = dataFilter.ToList();

        return new Result { Orders = result, TotalCount = result.Count };
    }
}