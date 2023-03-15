using Blazored.LocalStorage;
using Blazored.SessionStorage;
using IdentityModel.OidcClient;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Reflection;
using YomLog.Shared.Extensions;
using MudBlazor;

namespace YomLog.BlazorShared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfiguration config)
    {
        services.AddSingleton(sp =>
        {
            var httpClient =
                config.HttpMessageHandler != null
                ? new HttpClient(config.HttpMessageHandler) : new HttpClient();
            httpClient.BaseAddress = config.AppSettings.ApiBaseAddress;
            httpClient.Timeout = TimeSpan.FromSeconds(130);

            return httpClient;
        });

        services.AddMudServices(mudConfig =>
        {
            mudConfig.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            mudConfig.SnackbarConfiguration.PreventDuplicates = false;
            mudConfig.SnackbarConfiguration.NewestOnTop = true;
            mudConfig.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            mudConfig.SnackbarConfiguration.ShowTransitionDuration = 200;

            config.MudServicesConfiguration(mudConfig);
        });

        // Add auth services
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();

        if (config.OidcClientOptions != null)
        {
            services.AddTransient(sp => new OidcClient(config.OidcClientOptions));
            services.AddScoped<IAuthService, OidcAuthService>();
        }

        // Features
        // Blazor特有の機能 (JSRuntime, NavigationManagerなど) を含むサービスは、Scoped or TransientでDIすること
        // (Singletonだと起動不可)
        services.AddTransient(sp => config.AppSettings);

        services.AddBlazoredLocalStorage();
        services.AddBlazoredSessionStorage();

        // Add services automatically
        var assemblyNames =
            new List<Assembly> { Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() }
                .Select(x => x.GetName().Name!);
        var assemblies = Assembly.GetCallingAssembly().CollectReferencedAssemblies(assemblyNames);

        services.AddByAttribute(assemblies);

        return services;
    }
}

public class AppConfiguration
{
    public AppSettings AppSettings { get; set; } = new();
    public Action<MudServicesConfiguration> MudServicesConfiguration { get; set; } = _ => { };
    public OidcClientOptions? OidcClientOptions { get; set; }
    public HttpMessageHandler? HttpMessageHandler { get; set; }
}