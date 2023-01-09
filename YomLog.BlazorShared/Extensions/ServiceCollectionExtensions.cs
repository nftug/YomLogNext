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

namespace YomLog.BlazorShared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices<TEnvironment, TPreference, TCache, TPopup>
        (this IServiceCollection services, AppConfiguration config)
        where TEnvironment : class, IEnvironmentHelper
        where TPreference : class, IPreferenceRepositoryService
        where TCache : class, ICacheRepositoryService
        where TPopup : class, IPopupService
    {
        services.AddSingleton(sp =>
            new HttpClient { BaseAddress = config.AppSettings.ApiBaseAddress, Timeout = TimeSpan.FromSeconds(30) }
        );

        services.AddMudServices(config.MudServicesConfiguration);

        AddAuthServices(services, config.OidcClientOptions);

        // Features
        // Blazor特有の機能 (JSRuntime, NavigationManagerなど) を含むサービスは、Scoped or TransientでDIすること
        // (Singletonだと起動不可)
        services.AddTransient(sp => config.AppSettings);
        services.AddTransient<IPreferenceRepositoryService, TPreference>();
        services.AddTransient<ICacheRepositoryService, TCache>();
        services.AddTransient<IPopupService, TPopup>();
        services.AddScoped<LayoutService>();
        services.AddScoped<PageInfoService>();
        services.AddScoped<HttpClientWrapper>();
        services.AddScoped<IEnvironmentHelper, TEnvironment>();
        services.AddScoped<ScrollInfoService>();

        services.AddBlazoredLocalStorage();
        services.AddBlazoredSessionStorage();

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfiguration config)
        => AddAppServices<EnvironmentHelper, BlazorPreferenceRepository, BlazorCacheRepository, BlazorPopupService>
            (services, config);

    private static void AddAuthServices(this IServiceCollection services, OidcClientOptions options)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddTransient(sp => new OidcClient(options));
        services.AddScoped<AuthenticationStateProvider, MyAuthenticationStateProvider>();
        services.AddScoped<OidcAuthService>();
    }
}

public class AppConfiguration
{
    public AppSettings AppSettings { get; set; } = new();
    public Action<MudServicesConfiguration> MudServicesConfiguration { get; set; } = _ => { };
    public OidcClientOptions OidcClientOptions { get; set; } = new();
}