using Reporting.Application.Common.Enums;

namespace Reporting.Application.Common.Models;

public class RequestBase
{
    public RestaurantType Type { get; set; }
    public int PageIndex { get; set; } = 1;
}
