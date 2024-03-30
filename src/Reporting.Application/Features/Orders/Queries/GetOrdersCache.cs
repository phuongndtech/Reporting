using MediatR;
using Reporting.Domain.Entity;
using Microsoft.Extensions.Caching.Memory;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;

namespace Reporting.Application.Features.Orders.Queries;

public class GetOrdersCache
{
    public class Query: IRequest<Result>
    {
        public RestaurantType RestaurantType { get; set; }
    }

    public class Result
    {
        public List<Order>? Orders { get; set; }
    }

    public class Handler(IMemoryCache memoryCache, IExcelReader excelReader) : IRequestHandler<Query, Result>
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        private readonly IExcelReader _excelReader = excelReader;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            string cacheKey = $"Orders_{request.RestaurantType}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Order>? orders))
            {
                orders = await _excelReader.GetOrdersAsync(request.RestaurantType);

                _memoryCache.Set(cacheKey, orders);
            }

            return new Result { Orders = orders ?? [] };
        }
    }
}