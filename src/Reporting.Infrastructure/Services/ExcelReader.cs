using Aspose.Cells;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;
using Reporting.Domain.Entity;

namespace Reporting.Infrastructure.Services;

public class ExcelReader : IExcelReader
{
    public Task<List<Order>> GetOrdersAsync(RestaurantType type)
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
                ProductPrice = Convert.ToDecimal(worksheet.Cells[row, 4].Value),
                TotalProducts = Convert.ToInt32(worksheet.Cells[row, 5].Value)
            };
            results.Add(order);
        }

        return Task.FromResult(results);
    }

    public Task<List<ProductPrice>> GetProductPricesAsync(RestaurantType type)
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

    public void PopulateFile(Worksheet sheet, List<Order> orders)
    {
        sheet.Cells["A1"].PutValue("Order Number");
        sheet.Cells["B1"].PutValue("Order Date");
        sheet.Cells["C1"].PutValue("Item Name");
        sheet.Cells["D1"].PutValue("Quantity");
        sheet.Cells["E1"].PutValue("Product Price");
        sheet.Cells["F1"].PutValue("Total Products");

        int row = 2;
        foreach (var order in orders)
        {
            sheet.Cells[$"A{row}"].PutValue(order.OrderNumber);
            sheet.Cells[$"B{row}"].PutValue(order.OrderDate.ToString("yyyy-MM-dd"));
            sheet.Cells[$"C{row}"].PutValue(order.ItemName);
            sheet.Cells[$"D{row}"].PutValue(order.Quantity);
            sheet.Cells[$"E{row}"].PutValue(order.ProductPrice);
            sheet.Cells[$"F{row}"].PutValue(order.TotalProducts);
            row++;
        }
    }
}