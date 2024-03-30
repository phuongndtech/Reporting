namespace Reporting.Domain.Entity.Charts;

public class RevenuePeriodChart
{
    public List<Period>? Period1 { get; set; }
    public List<Period>? Period2 { get; set; }
    public List<Period>? Period3 { get; set; }
    public List<Period>? Period4 { get; set; }
}

public struct Period
{
    public string RestaurantName { get; set; }
    public decimal Percentage { get; set; }
}