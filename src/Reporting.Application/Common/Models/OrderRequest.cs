namespace Reporting.Application.Common.Models;

public class OrderRequest: RequestBase
{
    public double? ProductPrice { get; set; }
    public int? OrderNumber { get; set; }
}