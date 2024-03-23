using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reporting.Application.Common.Interfaces;
using Reporting.Infrastructure.Persistence;
using Reporting.Infrastructure.Services;

namespace Reporting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(
                     configuration.GetConnectionString("DefaultConnection"),
                     b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

        services.AddScoped<IExcelReader, ExcelReader>();

        return services;
    }
}