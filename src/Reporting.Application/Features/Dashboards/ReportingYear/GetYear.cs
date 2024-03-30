using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Features.Orders.Queries;

namespace Reporting.Application.Features.Dashboards.ReportingYear;

public class GetYear
{
    public class Query: IRequest<int>;


    public class Handler(IMediator mediator) : IRequestHandler<Query, int>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<int> Handle(Query request, CancellationToken cancellationToken)
        {
            var restaurantOneOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.One }, cancellationToken);

            var restaurantTwoOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.Two }, cancellationToken);

            if (restaurantOneOrderData.Orders == null || restaurantTwoOrderData.Orders == null)
            {
                throw new ArgumentNullException("Oops! Data not found");
            }

            var year = 0;

            foreach (var order in restaurantOneOrderData.Orders.Concat(restaurantTwoOrderData.Orders))
            {
                if (order.OrderDate.Year > year)
                {
                    year = order.OrderDate.Year;
                }
            }

            if (year <= 0)
            {
                throw new ArgumentException("No orders found for any year in the file");
            }

            return year;
        }
    }
}
