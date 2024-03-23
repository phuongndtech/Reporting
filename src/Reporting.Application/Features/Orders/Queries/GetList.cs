using MediatR;
using Reporting.Application.Common.Interfaces;
using Reporting.Application.Common.Models;
using Reporting.Domain.Entity;
using static Reporting.Application.Features.Orders.Queries.GetList;

namespace Reporting.Application.Features.Orders.Queries;

public class GetList
{
    public class Query : RequestBase, IRequest<Result>;

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

        var data = orders.Skip(request.PageNumber).Take(request.PageSize).ToList();

        return new Result { Orders = data, TotalCount = orders.Count };
    }
}