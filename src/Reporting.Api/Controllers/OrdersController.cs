using Aspose.Cells;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;
using Reporting.Application.Common.Models;
using Reporting.Application.Features.Orders.Queries;
using Reporting.Domain.Entity;

namespace Reporting.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator, IExcelReader excelReader) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    private readonly IExcelReader _excelReader = excelReader;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] OrderRequest request)
    {
        var result = await _mediator.Send(new GetList.Query
        {
            Type = request.Type,
            OrderNumber = request.OrderNumber,
            SearchText = request.SearchText
        });

        return Ok(result.Orders.OrderBy(x=>x.OrderNumber));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var restaurantOneOrderData = await _excelReader.GetOrders(RestaurantType.One);
        var restaurantTwoOrderData = await _excelReader.GetOrders(RestaurantType.Two);

        Workbook workbook = new();

        Worksheet restaurantOneSheet = workbook.Worksheets.Add("Restaurant 1");

        Worksheet restaurantTwoSheet = workbook.Worksheets.Add("Restaurant 2");

        PopulateSheetWithData(restaurantOneSheet, restaurantOneOrderData);

        PopulateSheetWithData(restaurantTwoSheet, restaurantTwoOrderData);

        MemoryStream stream = new();

        workbook.Save(stream, SaveFormat.Xlsx);

        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "restaurant_data.xlsx");
    }

    private static void PopulateSheetWithData(Worksheet sheet, List<Order> orders)
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