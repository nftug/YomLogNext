using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string appDataPath)
    {
        SQLitePCL.Batteries_V2.Init();
        services.AddScoped(_ => new DataContext(appDataPath));

        var assemblies = new[] { System.Reflection.Assembly.GetExecutingAssembly() };
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "Repository");
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "Service");
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "Factory");

        return services;
    }

    public static IServiceProvider UseInfrastructure(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.Migrate();
        return serviceProvider;
    }
}
