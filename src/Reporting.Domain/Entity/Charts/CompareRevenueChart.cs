namespace Reporting.Domain.Entity.Charts;

public class CompareRevenueChart
{
    public required List<YearRevenue> Restaurant1 { get; set; }
    public required List<YearRevenue> Restaurant2 { get; set; }
}

public struct YearRevenue
{
    public int Year { get; set; }
    public decimal Revenue { get; set; }
}