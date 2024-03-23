using System.Diagnostics;

namespace Reporting.Domain.Entity;

public class Order
{
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public double ProductPrice { get; set; }
    public int TotalProducts { get; set; }
}