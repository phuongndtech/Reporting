using Aspose.Cells;
using Reporting.Application.Common.Enums;
using Reporting.Domain.Entity;

namespace Reporting.Application.Common.Interfaces;

public interface IExcelReader
{
    Task<List<Order>> GetOrdersAsync(RestaurantType type);

    Task<List<ProductPrice>> GetProductPricesAsync(RestaurantType type);

    void PopulateFile(Worksheet worksheet, List<Order> orders);
}