using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;
using Reporting.Domain.Entity;
using static Reporting.Application.Features.Orders.Queries.GetList;

namespace Reporting.Application.Features.Orders.Queries;

public class GetList
{
    public class Query: IRequest<Result>
    {
        public RestaurantType RestaurantType { get; set; }
    }

    public class Result
    {
        public List<Order> Orders { get; set; }
    }
}

public class Handler(IExcelReader excelReader) : IRequestHandler<Query, Result>
{
    private readonly IExcelReader _excelReader = excelReader;

    public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
    {
        var orders = await _excelReader.GetOrders(request.RestaurantType);

        return new Result { Orders = orders };
    }
}