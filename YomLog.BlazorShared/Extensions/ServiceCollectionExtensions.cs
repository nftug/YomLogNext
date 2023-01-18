using Blazored.LocalStorage;
using Blazored.SessionStorage;
using IdentityModel.OidcClient;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Services.Auth;
using YomLog.BlazorShared.Services.Popup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using YomLog.BlazorShared.Services.Repository;
using System.Reflection;
using YomLog.Shared.Extensions;

namespace YomLog.BlazorShared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfiguration config)
    {
        services.AddSingleton(sp =>
            new HttpClient { BaseAddress = config.AppSettings.ApiBaseAddress, Timeout = TimeSpan.FromSeconds(30) }
        );

        services.AddMudServices(config.MudServicesConfiguration);

        AddAuthServices(services, config.OidcClientOptions);

        // Features
        // Blazor特有の機能 (JSRuntime, NavigationManagerなど) を含むサービスは、Scoped or TransientでDIすること
        // (Singletonだと起動不可)
        config.AppSettings.OidcClientOptions = config.OidcClientOptions;
        services.AddTransient(sp => config.AppSettings);
        services.AddScoped<LayoutService>();
        services.AddScoped<PageInfoService>();
        services.AddScoped<HttpClientWrapper>();
        services.AddScoped<ScrollInfoService>();
        services.AddScoped<ExceptionHubService>();

        services.AddBlazoredLocalStorage();
        services.AddBlazoredSessionStorage();

        // Add services automatically
        var assemblyNames =
            new List<Assembly> { Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() }
                .Select(x => x.GetName().Name!);
        var assemblies = Assembly.GetCallingAssembly().CollectReferencedAssemblies(assemblyNames);

        services.AddAssemblyTypes<IPreferenceRepositoryService>(assemblies, ServiceLifetime.Transient);
        services.AddAssemblyTypes<ICacheRepositoryService>(assemblies, ServiceLifetime.Transient);
        services.AddAssemblyTypes<IPopupService>(assemblies, ServiceLifetime.Transient);
        services.AddAssemblyTypes<IEnvironmentHelper>(assemblies, ServiceLifetime.Transient);
        services.AddAssemblyTypes<IDebugLoggerService>(assemblies, ServiceLifetime.Transient);
        services.AddAssemblyTypes(assemblies, ServiceLifetime.Transient, "ApiService");

        return services;
    }

    private static void AddAuthServices(this IServiceCollection services, OidcClientOptions options)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddTransient(sp => new OidcClient(options));
        services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();
        services.AddScoped<IAuthService, OidcAuthService>();
    }
}

public class AppConfiguration
{
    public AppSettings AppSettings { get; set; } = new();
    public Action<MudServicesConfiguration> MudServicesConfiguration { get; set; } = _ => { };
    public OidcClientOptions OidcClientOptions { get; set; } = new();
}