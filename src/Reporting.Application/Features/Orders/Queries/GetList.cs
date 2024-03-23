using MediatR;
using Reporting.Application.Common.Enums;
using static Reporting.Application.Features.Orders.Queries.GetList;

namespace Reporting.Application.Features.Orders.Queries;

public class GetList
{
    public class Query: IRequest<List<Result>>
    {
        public RestaurantType RestaurantType { get; set; }
    }

    public class Result
    {

    }
}

public class Handler : IRequestHandler<GetList.Query, List<GetList.Result>>
{
    public Handler() { }

    public Task<List<Result>> Handle(Query request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}