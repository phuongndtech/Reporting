using Reporting.Application.Common.Enums;
using Reporting.Domain.Entity;

namespace Reporting.Application.Common.Interfaces;

public interface IExcelReader
{
    Task<List<Order>> GetOrders(RestaurantType type);
    Task<List<ProductPrice>> GetProductPrices(RestaurantType type);
}