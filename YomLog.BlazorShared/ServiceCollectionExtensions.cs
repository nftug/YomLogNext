using Blazored.LocalStorage;
using Blazored.SessionStorage;
using IdentityModel.OidcClient;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Auth;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
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
            config.MudServicesConfiguration?.Invoke(mudConfig);
        });

        // Add auth services
        services.AddAuthorizationCore();

        if (config.OidcClientOptions != null)
        {
            services.AddTransient(sp => new OidcClient(config.OidcClientOptions));
            services.AddScoped<IAuthService, OidcAuthService>();
        }

        services.AddTransient(sp => config.AppSettings);

        services.AddBlazoredLocalStorage();
        services.AddBlazoredSessionStorage();

        // Add services automatically
        services.AddFromCurrentAssembly();

        return services;
    }
}

public record AppConfiguration(
    AppSettings AppSettings,
    Action<MudServicesConfiguration>? MudServicesConfiguration,
    OidcClientOptions? OidcClientOptions,
    HttpMessageHandler? HttpMessageHandler
);