using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Features.Orders.Queries;
using Reporting.Domain.Entity.Charts;

namespace Reporting.Application.Features.Dashboards.Charts.CompareRevenueChart;

public class GetChart
{
    public class Query : IRequest<Domain.Entity.Charts.CompareRevenueChart>;

    public class Handler(IMediator mediator) : IRequestHandler<Query, Domain.Entity.Charts.CompareRevenueChart>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<Domain.Entity.Charts.CompareRevenueChart> Handle(Query request, CancellationToken cancellationToken)
        {
            var restaurantOneOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.One }, cancellationToken);

            var restaurantTwoOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.Two }, cancellationToken);

            if (restaurantOneOrderData.Orders == null || restaurantTwoOrderData.Orders == null)
            {
                throw new ArgumentNullException("Oops! Data not found");
            }

            var allOrders = restaurantOneOrderData.Orders.Concat(restaurantTwoOrderData.Orders);

            var listYears = allOrders.Select(x => x.OrderDate.Year).Distinct().ToList();

            if (listYears.Count == 0)
            {
                throw new ArgumentException("Don't have year in the data");
            }

            var compareRevenueChart = new Domain.Entity.Charts.CompareRevenueChart
            {
                Restaurant1 = [],
                Restaurant2 = []
            };

            foreach (var year in listYears)
            {
                var revenueForYearRestaurantOne = restaurantOneOrderData.Orders
                    .Where(order => order.OrderDate.Year == year)
                    .Sum(order => order.Quantity * order.ProductPrice);

                var revenueForYearRestaurantTwo = restaurantTwoOrderData.Orders
                    .Where(order => order.OrderDate.Year == year)
                    .Sum(order => order.Quantity * order.ProductPrice);

                compareRevenueChart.Restaurant1.Add(new YearRevenue { Year = year, Revenue = revenueForYearRestaurantOne });
                compareRevenueChart.Restaurant2.Add(new YearRevenue { Year = year, Revenue = revenueForYearRestaurantTwo });
            }

            compareRevenueChart.Restaurant1 = [.. compareRevenueChart.Restaurant1.OrderBy(x => x.Year)];

            compareRevenueChart.Restaurant2 = [.. compareRevenueChart.Restaurant2.OrderBy(x => x.Year)];

            return compareRevenueChart;
        }
    }
}