using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Features.Orders.Queries;
using Reporting.Domain.Entity.Charts;

namespace Reporting.Application.Features.Dashboards.Charts.RevenuePeriodChart;

public class GetChart
{
    public class Query: IRequest<Domain.Entity.Charts.RevenuePeriodChart>;

    public class Handler(IMediator mediator) : IRequestHandler<Query, Domain.Entity.Charts.RevenuePeriodChart>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<Domain.Entity.Charts.RevenuePeriodChart> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var restaurantOneOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.One }, cancellationToken);

            var restaurantTwoOrderData = await _mediator.Send(new GetOrdersCache.Query { RestaurantType = RestaurantType.Two }, cancellationToken);

            if (restaurantOneOrderData.Orders == null || restaurantTwoOrderData.Orders == null)
            {
                throw new ArgumentNullException("Oops! Data not found");
            }

            var currentYear = 0;

            foreach (var order in restaurantOneOrderData.Orders.Concat(restaurantTwoOrderData.Orders))
            {
                if (order.OrderDate.Year > currentYear)
                {
                    currentYear = order.OrderDate.Year;
                }
            }

            if (currentYear <= 0)
            {
                throw new ArgumentNullException("No orders found for any year in the file");
            }

            var revenuePeriodChart = new Domain.Entity.Charts.RevenuePeriodChart();

            for (int quarter = 1; quarter <= 4; quarter++)
            {
                var startDate = new DateTime(currentYear, ((quarter - 1) * 3) + 1, 1);
                var endDate = startDate.AddMonths(3).AddDays(-1);

                var restaurantOneRevenue = restaurantOneOrderData.Orders
                    .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                    .Sum(x => x.Quantity * x.ProductPrice);

                var restaurantTwoRevenue = restaurantTwoOrderData.Orders
                    .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                    .Sum(x => x.Quantity * x.ProductPrice);

                var totalRevenue = restaurantOneRevenue + restaurantTwoRevenue;

                var restaurantOnePercentage = totalRevenue != 0 ? (restaurantOneRevenue / totalRevenue * 100) : 0;

                var restaurantTwoPercentage = 100 - restaurantOnePercentage;

                GetChartData(revenuePeriodChart, quarter, restaurantOnePercentage, restaurantTwoPercentage);
            }

            return revenuePeriodChart;
        }

        private static void GetChartData(
            Domain.Entity.Charts.RevenuePeriodChart quartersRevenue,
            int quarter,
            decimal restaurantOnePercentage,
            decimal restaurantTwoPercentage)
        {
            switch (quarter)
            {
                case 1:
                    quartersRevenue.Period1 =
                [
                    new Period { RestaurantName = "Restaurant One", Percentage = restaurantOnePercentage },
                    new Period { RestaurantName = "Restaurant Two", Percentage = restaurantTwoPercentage }
                ];
                    break;
                case 2:
                    quartersRevenue.Period2 =
                [
                    new Period { RestaurantName = "Restaurant One", Percentage = restaurantOnePercentage },
                    new Period { RestaurantName = "Restaurant Two", Percentage = restaurantTwoPercentage }
                ];
                    break;
                case 3:
                    quartersRevenue.Period3 =
                [
                    new Period { RestaurantName = "Restaurant One", Percentage = restaurantOnePercentage },
                    new Period { RestaurantName = "Restaurant Two", Percentage = restaurantTwoPercentage }
                ];
                    break;
                case 4:
                    quartersRevenue.Period4 =
                [
                    new Period { RestaurantName = "Restaurant One", Percentage = restaurantOnePercentage },
                    new Period { RestaurantName = "Restaurant Two", Percentage = restaurantTwoPercentage }
                ];
                    break;
            }
        }
    }
}