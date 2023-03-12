using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor;
using YomLog.BlazorShared;
using YomLog.Domain;
using YomLog.Infrastructure;
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
        string oidcClientId = config.GetRequiredSection("Oidc").GetValue<string>("ClientId")!;
        string oidcAuthority = config.GetRequiredSection("Oidc").GetValue<string>("Authority")!;

        builder.Services.AddAppServices(new()
        {
            MudServicesConfiguration = config =>
            {
                config.SnackbarConfiguration.ShowCloseIcon = false;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
            },
            OidcClientOptions = new()
            {
                Authority = oidcAuthority,
                ClientId = oidcClientId,
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
            },
            HttpMessageHandler = GetInsecureHttpHandler()
        });

        builder.Services.AddInfrastructure(FileSystem.AppDataDirectory);
        builder.Services.AddDomainServices();

        var app = builder.Build();
        app.Services.UseInfrastructure();

        return app;
    }

    private static HttpMessageHandler GetInsecureHttpHandler()
    {
#if ANDROID
        var handler = new Platforms.Android.CustomAndroidMessageHandler();
#else
        var handler = new HttpClientHandler();
#endif
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        return handler;
    }
}
