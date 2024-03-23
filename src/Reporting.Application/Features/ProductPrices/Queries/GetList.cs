using MediatR;
using Reporting.Application.Common.Interfaces;
using Reporting.Application.Common.Models;
using Reporting.Domain.Entity;
using static Reporting.Application.Features.ProductPrices.Queries.GetList;

namespace Reporting.Application.Features.ProductPrices.Queries;

public class GetList
{
    public class Query: RequestBase, IRequest<Result>;

    public class Result
    {
        public List<ProductPrice> ProductPrices { get; set; }
    }
}

public class Handler(IExcelReader excelReader) : IRequestHandler<Query, Result>
{
    private readonly IExcelReader _excelReader = excelReader;

    public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
    {
        var productPrices = await _excelReader.GetProductPrices(request.Type);

        var data = productPrices.Skip(request.PageNumber).Take(request.PageSize).ToList();

        return new Result { ProductPrices = data };
    }
}