using Microsoft.Extensions.DependencyInjection;
using YomLog.Shared.Extensions;

namespace YomLog.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
        => services.AddByAttribute();
}
