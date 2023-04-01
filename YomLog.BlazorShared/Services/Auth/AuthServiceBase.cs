using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.BlazorShared.Services.Auth;

public abstract class AuthServiceBase : BindableBase, IAuthService
{
    protected readonly MyAuthenticationStateProvider _authStateProvider;
    protected readonly IPopupService _popupService;
    protected readonly NavigationManager _navigationManager;
    protected readonly ISnackbar _snackbar;
    protected readonly ExceptionHubService _exceptionHub;

    protected ReactivePropertySlim<bool> IsTokenValid { get; }
    protected ReadOnlyReactivePropertySlim<bool> IsIdentityAuthenticated { get; }
    public ReadOnlyReactivePropertySlim<bool> IsAuthenticated { get; }

    protected ReactiveTimer RefreshTimer { get; }
    protected virtual TimeSpan RefreshTimeSpan => TimeSpan.FromSeconds(30);

    public AuthServiceBase(
        AuthenticationStateProvider authStateProvider,
        IPopupService popupService,
        NavigationManager navigationManager,
        ISnackbar snackbar,
        ExceptionHubService exceptionHub
    )
    {
        _authStateProvider = (MyAuthenticationStateProvider)authStateProvider;
        _popupService = popupService;
        _navigationManager = navigationManager;
        _snackbar = snackbar;
        _exceptionHub = exceptionHub;

        IsTokenValid = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsIdentityAuthenticated = _authStateProvider.Identity
            .ObserveProperty(x => x.Value.IsAuthenticated)
            .ToReadOnlyReactivePropertySlim()
            .AddTo(Disposable);
        IsAuthenticated = Observable
            .Merge(IsIdentityAuthenticated, IsTokenValid)
            .Select(x => IsIdentityAuthenticated.Value && IsTokenValid.Value)
            .ToReadOnlyReactivePropertySlim()
            .AddTo(Disposable);

        RefreshTimer = new ReactiveTimer(RefreshTimeSpan).AddTo(Disposable);
        RefreshTimer.Subscribe(async _ => await _exceptionHub.Handle(RefreshTokenAsync));

        // If authenticated by authStateProvider, start refresh token immediately to switch IsTokenRefreshed flag into true.
        IsIdentityAuthenticated
            .Where(v => v)
            .Subscribe(_ => RefreshTimer.Start())
            .AddTo(Disposable);
        IsIdentityAuthenticated
            .Skip(1).Where(v => !v)
            .Subscribe(_ => RefreshTimer.Stop())
            .AddTo(Disposable);
    }

    public abstract Task LoginAsync(string? redirectTo = null);

    public abstract Task ProcessLoginCallbackAsync(string data);

    public virtual async Task LogoutAsync(bool forceLogout = false)
    {
        if (!forceLogout)
        {
            bool answer = await _popupService.ShowConfirm
                ("ログアウト", "現在のセッションからログアウトしますか？", okText: "ログアウト");
            if (!answer) return;
        }

        await OnAfterLogoutAsync();

        await _authStateProvider.MarkUserAsLoggedOut();
        IsTokenValid.Value = false;

        _navigationManager.NavigateTo("");
        _snackbar.Add("ログアウトしました。", Severity.Info);
    }

    public abstract Task RefreshTokenAsync();

    protected virtual async Task LoginCoreAsync(TokenModel tokenModel, string? redirectTo)
    {
        await _authStateProvider.MarkUserAsAuthenticated(tokenModel);
        IsTokenValid.Value = true;

        if (redirectTo != null)
            _navigationManager.NavigateTo(redirectTo, replace: true);

        _snackbar.Add("ログインしました。", Severity.Info);
    }

    protected virtual Task OnAfterLogoutAsync() => Task.CompletedTask;

    public Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateProvider.GetAuthenticationStateAsync();

    public Task<TokenModel?> GetTokenModelAsync() => _authStateProvider.GetTokenModelAsync();
}

public interface IAuthService
{
    ReadOnlyReactivePropertySlim<bool> IsAuthenticated { get; }

    Task LoginAsync(string? redirectTo = null);
    Task ProcessLoginCallbackAsync(string data);
    Task LogoutAsync(bool forceLogout = false);
    Task RefreshTokenAsync();
}
