using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Features.Orders.Queries;
using Reporting.Domain.Entity.Charts;

namespace Reporting.Application.Features.Dashboards.Charts.TopProductRevenueChart;

public class GetChart
{
    public class Query : IRequest<Domain.Entity.Charts.TopProductRevenueChart>;

    public class Handler(IMediator mediator) : IRequestHandler<Query, Domain.Entity.Charts.TopProductRevenueChart>
    {
        private readonly IMediator _mediator = mediator;

        public async Task<Domain.Entity.Charts.TopProductRevenueChart> Handle(Query request, CancellationToken cancellationToken)
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
                throw new ArgumentException("No orders found for any year in the file");
            }

            var orders = restaurantOneOrderData.Orders
                .Concat(restaurantTwoOrderData.Orders)
                .Where(order => order.OrderDate.Year == currentYear);

            var productRevenueMap = new Dictionary<string, decimal>();

            foreach (var order in orders)
            {
                if (productRevenueMap.ContainsKey(order.ItemName))
                {
                    productRevenueMap[order.ItemName] += order.Quantity * order.ProductPrice;
                }
                else
                {
                    productRevenueMap[order.ItemName] = order.Quantity * order.ProductPrice;
                }
            }

            var topProducts = productRevenueMap.OrderByDescending(pair => pair.Value)
                                                .Take(5)
                                                .Select(pair => new ProductRevenue { ProductName = pair.Key, Revenue = pair.Value })
                                                .ToList();

            return new Domain.Entity.Charts.TopProductRevenueChart { ProductRevenues = topProducts };
        }
    }
}
