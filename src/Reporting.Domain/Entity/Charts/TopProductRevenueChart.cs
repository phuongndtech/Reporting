namespace Reporting.Domain.Entity.Charts;

public class TopProductRevenueChart
{
    public List<ProductRevenue>? ProductRevenues { get; set; }
}

public struct ProductRevenue
{
    public string ProductName { get; set; }
    public decimal Revenue { get; set; }
}