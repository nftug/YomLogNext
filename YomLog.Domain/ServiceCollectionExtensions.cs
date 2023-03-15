using Microsoft.Extensions.DependencyInjection;
using YomLog.Shared.Extensions;

namespace YomLog.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddByAttribute();

        /*
        var assemblies = new[] { System.Reflection.Assembly.GetExecutingAssembly() };
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "Service");
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "Factory");
        */
        return services;
    }
}
