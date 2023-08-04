using Microsoft.Extensions.DependencyInjection;
using YomLog.Infrastructure.Shared.Services;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string appDataPath)
    {
        services.AddSingleton(sp => new AppConfig(appDataPath));
        services.AddFromCurrentAssembly();
        return services;
    }
}
