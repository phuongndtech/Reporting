using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Reporting.Domain.Entity;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardsController(IExcelReader excel, IMemoryCache memoryCache) : ControllerBase
{
    private readonly IExcelReader _excelReader = excel;

    private readonly IMemoryCache _memoryCache = memoryCache;

    [HttpGet("revenue-period")]
    public async Task<IActionResult> GetRevenueByPeriod()
    {
        var restaurantOneOrderData = await GetCachedOrders(RestaurantType.One);

        var restaurantTwoOrderData = await GetCachedOrders(RestaurantType.Two);

        var currentYear = 0;

        foreach (var order in restaurantOneOrderData.Concat(restaurantTwoOrderData))
        {
            if (order.OrderDate.Year > currentYear)
            {
                currentYear = order.OrderDate.Year;
            }
        }

        if (currentYear <= 0)
            return NotFound("No orders found for any year in the file.");

        var quartersRevenue = new RevenuePeriodChart();

        for (int quarter = 1; quarter <= 4; quarter++)
        {
            var startDate = new DateTime(currentYear, ((quarter - 1) * 3) + 1, 1);
            var endDate = startDate.AddMonths(3).AddDays(-1);

            var restaurantOneRevenue = restaurantOneOrderData
                .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                .Sum(x => x.Quantity * x.ProductPrice);

            var restaurantTwoRevenue = restaurantTwoOrderData
                .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                .Sum(x => x.Quantity * x.ProductPrice);

            var totalRevenue = restaurantOneRevenue + restaurantTwoRevenue;

            var restaurantOnePercentage = totalRevenue != 0 ? (restaurantOneRevenue / totalRevenue * 100) : 0;

            var restaurantTwoPercentage = 100 - restaurantOnePercentage;

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

        return Ok(quartersRevenue);
    }

    [HttpGet("current-year")]
    public async Task<IActionResult> GetCurrentYear()
    {
        var restaurantOneOrderData = await GetCachedOrders(RestaurantType.One);

        var restaurantTwoOrderData = await GetCachedOrders(RestaurantType.Two);

        var currentYear = 0;

        foreach (var order in restaurantOneOrderData.Concat(restaurantTwoOrderData))
        {
            if (order.OrderDate.Year > currentYear)
            {
                currentYear = order.OrderDate.Year;
            }
        }

        if (currentYear <= 0)
            return NotFound("No orders found for any year in the file.");

        return Ok(currentYear);
    }

    [HttpGet("top-product")]

    public async Task<IActionResult> GetTopProductByRevenue()
    {
        var restaurantOneOrderData = await GetCachedOrders(RestaurantType.One);

        var restaurantTwoOrderData = await GetCachedOrders(RestaurantType.Two);

        var currentYear = 0;

        foreach (var order in restaurantOneOrderData.Concat(restaurantTwoOrderData))
        {
            if (order.OrderDate.Year > currentYear)
            {
                currentYear = order.OrderDate.Year;
            }
        }

        if (currentYear <= 0)
            return NotFound("No orders found for any year in the file.");

        var orders = restaurantOneOrderData
            .Concat(restaurantTwoOrderData)
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

        var chartData = new TopProductRevenueChart { ProductRevenues = topProducts };

        return Ok(chartData);
    }

    [HttpGet("restaurant-revenue")]
    public async Task<IActionResult> GetRestaurantRevenue()
    {
        var restaurantOneOrderData = await GetCachedOrders(RestaurantType.One);

        var restaurantTwoOrderData = await GetCachedOrders(RestaurantType.Two);

        var allOrders = restaurantOneOrderData.Concat(restaurantTwoOrderData);

        var listYears = allOrders.Select(x => x.OrderDate.Year).Distinct().ToList();

        if (listYears.Count == 0)
            return NotFound("Don't have year in the data");

        var compareRevenue = new CompareRevenueChart
        {
            Restaurant1 = [],
            Restaurant2 = []
        };

        foreach (var year in listYears)
        {
            var revenueForYearRestaurantOne = restaurantOneOrderData
                .Where(order => order.OrderDate.Year == year)
                .Sum(order => order.Quantity * order.ProductPrice);

            var revenueForYearRestaurantTwo = restaurantTwoOrderData
                .Where(order => order.OrderDate.Year == year)
                .Sum(order => order.Quantity * order.ProductPrice);

            compareRevenue.Restaurant1.Add(new YearRevenue { Year = year, Revenue = revenueForYearRestaurantOne });
            compareRevenue.Restaurant2.Add(new YearRevenue { Year = year, Revenue = revenueForYearRestaurantTwo });
        }

        compareRevenue.Restaurant1 = [.. compareRevenue.Restaurant1.OrderBy(x => x.Year)];

        compareRevenue.Restaurant2 = [.. compareRevenue.Restaurant2.OrderBy(x => x.Year)];

        return Ok(compareRevenue);
    }

    private async Task<List<Order>> GetCachedOrders(RestaurantType restaurantType)
    {
        string cacheKey = $"Orders_{restaurantType}";

        if (!_memoryCache.TryGetValue(cacheKey, out List<Order> orders))
        {
            orders = await _excelReader.GetOrders(restaurantType);

            _memoryCache.Set(cacheKey, orders);
        }

        return orders;
    }
}

public class RevenuePeriodChart
{
    public List<Period> Period1 { get; set; }
    public List<Period> Period2 { get; set; }
    public List<Period> Period3 { get; set; }
    public List<Period> Period4 { get; set; }
}

public struct Period
{
    public string RestaurantName { get; set; }
    public decimal Percentage { get; set; }
}

public class TopProductRevenueChart
{
    public List<ProductRevenue> ProductRevenues { get; set; }
}

public struct ProductRevenue
{
    public string ProductName { get; set; }
    public decimal Revenue { get; set; }
}

public class CompareRevenueChart
{
    public List<YearRevenue> Restaurant1 { get; set; }
    public List<YearRevenue> Restaurant2 { get; set; }
}

public struct YearRevenue
{
    public int Year { get; set; }
    public decimal Revenue { get; set; }
}