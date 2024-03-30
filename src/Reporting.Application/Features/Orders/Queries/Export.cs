using Aspose.Cells;
using MediatR;
using Reporting.Application.Common.Enums;
using Reporting.Application.Common.Interfaces;

namespace Reporting.Application.Features.Orders.Queries;

public class Export
{
    public class Query : IRequest<Result>;

    public class Result
    {
        public required MemoryStream FileStream { get; set; }
    }

    public class Handler(IExcelReader excelReader) : IRequestHandler<Query, Result>
    {
        private readonly IExcelReader _excelReader = excelReader;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var restaurantOneOrderData = await _excelReader.GetOrdersAsync(RestaurantType.One);

            var restaurantTwoOrderData = await _excelReader.GetOrdersAsync(RestaurantType.Two);

            Workbook workbook = new();

            Worksheet restaurantOneSheet = workbook.Worksheets.Add("Restaurant 1");

            Worksheet restaurantTwoSheet = workbook.Worksheets.Add("Restaurant 2");

            _excelReader.PopulateFile(restaurantOneSheet, restaurantOneOrderData);

            _excelReader.PopulateFile(restaurantTwoSheet, restaurantTwoOrderData);

            Worksheet defaultSheet = workbook.Worksheets["Sheet1"];

            if (defaultSheet != null) workbook.Worksheets.RemoveAt(defaultSheet.Index);

            MemoryStream stream = new();

            workbook.Save(stream, SaveFormat.Xlsx);

            stream.Position = 0;

            return await Task.FromResult(new Result { FileStream = stream });
        }
    }
}
