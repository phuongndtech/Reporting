using Microsoft.Extensions.DependencyInjection;
using Reporting.Application.Common.Interfaces;
using Reporting.Infrastructure.Services;

namespace Reporting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IExcelReader, ExcelReader>();

        return services;
    }
}