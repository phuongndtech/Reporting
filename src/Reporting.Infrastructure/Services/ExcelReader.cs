using Aspose.Cells;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;
using Reporting.Domain.Entity;

namespace Reporting.Infrastructure.Services;

public class ExcelReader : IExcelReader
{
    public Task<List<Order>> GetOrders(RestaurantType type)
    {
        var fileName = $"restaurant-{(int)type}-orders.csv";

        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var folderPath = Path.Combine(currentDirectory, "ExternalData");

        var filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The excel file {fileName} was not found");
        }

        var results = new List<Order>();

        var workbook = new Workbook(filePath);

        var worksheet = workbook.Worksheets[0];

        int rows = worksheet.Cells.MaxDataRow + 1;

        for (int row = 1; row < rows; row++)
        {
            var order = new Order
            {
                OrderNumber = Convert.ToInt32(worksheet.Cells[row, 0].Value),
                OrderDate = Convert.ToDateTime(worksheet.Cells[row, 1].Value),
                ItemName = worksheet.Cells[row, 2].StringValue,
                Quantity = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                ProductPrice = Convert.ToDouble(worksheet.Cells[row, 4].Value),
                TotalProducts = Convert.ToInt32(worksheet.Cells[row, 5].Value)
            };
            results.Add(order);
        }

        return Task.FromResult(results);
    }

    public Task<List<ProductPrice>> GetProductPrices(RestaurantType type)
    {
        var fileName = $"restaurant-{(int)type}-products-price.csv";

        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var folderPath = Path.Combine(currentDirectory, "ExternalData");

        var filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The excel file {fileName} was not found");
        }

        var results = new List<ProductPrice>();

        var workbook = new Workbook(filePath);

        var worksheet = workbook.Worksheets[0];

        int rows = worksheet.Cells.MaxDataRow + 1;

        for (int row = 1; row < rows; row++)
        {
            var productPrice = new ProductPrice
            {
                ItemName = worksheet.Cells[row, 0].StringValue,
                Price = Convert.ToDouble(worksheet.Cells[row, 1].Value)
            };
            results.Add(productPrice);
        }

        return Task.FromResult(results);
    }
}