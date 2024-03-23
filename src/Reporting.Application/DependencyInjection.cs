using Microsoft.Extensions.DependencyInjection;

namespace Reporting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddServices();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));

        return services;
    }
}