using Reporting.Application.Common.Enums;

namespace Reporting.Application.Common.Models;

public class RequestBase
{
    public RestaurantType Type { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
