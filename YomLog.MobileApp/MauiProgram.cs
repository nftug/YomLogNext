using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor;
using YomLog.BlazorShared.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.MobileApp.Services;

namespace YomLog.MobileApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("YomLog.MobileApp.Properties.appsettings.json");
        var config = new ConfigurationBuilder().AddJsonStream(stream!).Build();
        builder.Configuration.AddConfiguration(config);

        // #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        // #endif

        string apiBaseUri = config.GetRequiredSection("ApiBaseUri").Get<string>()!;
        var oidcSettings = config.GetRequiredSection(nameof(OidcSettings)).Get<OidcSettings>();

        builder.Services.AddAppServices(new()
        {
            MudServicesConfiguration = config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            },
            OidcClientOptions = new()
            {
                Authority = oidcSettings?.Authority,
                ClientId = oidcSettings?.ClientId,
                Scope = "openid profile offline_access",
                RedirectUri = "yomlog://callback",
                PostLogoutRedirectUri = "yomlog://callback",
                Browser = new WebAuthenticatorBrowser()
            },
            AppSettings = new()
            {
                IsNativeApp = true,
                DefaultMaxWidth = MaxWidth.Small,
                AppName = AppInfo.Name,
                ApiBaseAddress = new Uri(apiBaseUri)
            }
        });

        return builder.Build();
    }
}
