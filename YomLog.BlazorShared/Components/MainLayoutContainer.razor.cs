using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using YomLog.BlazorShared.Services;
using MudBlazor;
using System.Reactive.Linq;
using YomLog.BlazorShared.Models;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Services.Auth;
using Reactive.Bindings;

namespace YomLog.BlazorShared.Components;

public partial class MainLayoutContainer : BindComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private ScrollInfoService ScrollInfoService { get; set; } = null!;
    [Inject] private OidcAuthService AuthService { get; set; } = null!;

    [Parameter] public bool? IsDarkModeDefault { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment? DrawerSection { get; set; }
    [Parameter] public RenderFragment AppBarSection { get; set; } = null!;
    [Parameter] public RenderFragment? NotFoundSection { get; set; }

    private ErrorBoundary? _errorBoundary;
    private MudThemeProvider? _mudThemeProvider;

    private ReactivePropertySlim<bool> IsInitializing { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        LayoutService.UserPreferences
            .Skip(1)
            .Subscribe(_ => StateHasChanged())
            .AddTo(Disposable);
        LayoutService.Page
            .Where(v => v != null)
            .Subscribe(_ => StateHasChanged())
            .AddTo(Disposable);

        IsInitializing = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsInitializing.Skip(1).Subscribe(_ => StateHasChanged());

        await ScrollInfoService.RegisterService();
        await ApplyUserPreferences();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        // Initializing application
        IsInitializing.Value = true;

        try
        {
            // Refresh token, and start automatic token renew after 30 secs
            if (AuthService.IsAuthenticated.Value)
            {
                await AuthService.RefreshTokenAsync();
                AuthService.StartRefreshTimer();
            }
        }
        finally
        {
            // Initialization finished
            IsInitializing.Value = false;
        }
    }

    protected override void OnParametersSet()
    {
        _errorBoundary?.Recover();
    }

    private async Task ApplyUserPreferences()
    {
        var defaultDarkMode = IsDarkModeDefault ?? await _mudThemeProvider!.GetSystemPreference();
        await LayoutService.ApplyUserPreferences(defaultDarkMode);
    }

    private void LayoutServiceOnMajorUpdateOccurred(object? sender, EventArgs e) => StateHasChanged();
}

public class InitializeLayoutEventArgs : EventArgs
{
    public bool IsAuthenticated { get; init; }
}