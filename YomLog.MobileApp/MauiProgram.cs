using Microsoft.Extensions.Logging;
using MudBlazor;
using YomLog.BlazorShared.Extensions;
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

        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();

        string uri = "https://localhost:5011/api/";

        builder.Services.AddAppServices<
            EnvironmentHelper,
            MauiPreferenceRepository,
            MauiCacheRepository,
            MauiPopupService
        >(new()
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
                Authority = "https://localhost:5022",
                ClientId = "YomLog.MobileApp",
                Scope = "openid profile name role offline_access",
                RedirectUri = "yomlog://",
                PostLogoutRedirectUri = "yomlog://",
                Browser = new WebAuthenticatorBrowser()
            },
            AppSettings = new()
            {
                IsNativeApp = true,
                DefaultMaxWidth = MaxWidth.Small,
                AppName = AppInfo.Name,
                ApiBaseAddress = new Uri(uri)
            }
        });

        return builder.Build();
    }
}
