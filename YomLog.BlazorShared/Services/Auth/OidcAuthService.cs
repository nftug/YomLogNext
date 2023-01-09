using System.Reactive.Linq;
using Blazored.SessionStorage;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.BlazorShared.Services.Auth;

public class OidcAuthService : BindBase
{
    private readonly MyAuthenticationStateProvider _authStateProvider;
    private readonly OidcClient _oidcClient;
    private readonly IPopupService _popupService;
    private readonly NavigationManager _navigationManager;
    private readonly ISessionStorageService _sessionStorage;
    private readonly ISnackbar _snackBar;

    private Timer? _timer;
    private readonly TimeSpan RefreshTimeSpan = TimeSpan.FromSeconds(30);

    public ReadOnlyReactivePropertySlim<bool> IsAuthenticated { get; }

    public OidcAuthService(
        AuthenticationStateProvider authStateProvider,
        OidcClient oidcClient,
        IPopupService popupService,
        NavigationManager navigationManager,
        ISessionStorageService sessionStorage,
        ISnackbar snackBar
    )
    {
        _authStateProvider = (MyAuthenticationStateProvider)authStateProvider;
        _oidcClient = oidcClient;
        _popupService = popupService;
        _navigationManager = navigationManager;
        _sessionStorage = sessionStorage;
        _snackBar = snackBar;

        IsAuthenticated = _authStateProvider.Identity
            .ObserveProperty(x => x.Value.IsAuthenticated)
            .ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
            .AddTo(Disposable);
    }

    public void StartRefreshTimer()
    {
        _timer = new(async (_) => await RefreshTokenAsync(), null, TimeSpan.FromSeconds(0), RefreshTimeSpan);
    }

    protected override void Dispose(bool disposing)
    {
        _timer?.Dispose();
    }

    public async Task LoginAsync()
    {
        if (_oidcClient.Options.Browser != null)
        {
            var loginResult = await _oidcClient.LoginAsync(new LoginRequest());
            await ProcessLoginResult(loginResult);
        }
        else
        {
            var state = await _oidcClient.PrepareLoginAsync();
            await _sessionStorage.SetItemAsync("State", state);
            _navigationManager.NavigateTo(state.StartUrl, forceLoad: true);
        }
    }

    public async Task ProcessLoginCallbackAsync(string data)
    {
        var state = await _sessionStorage.GetItemAsync<AuthorizeState>("State");
        var loginResult = await _oidcClient.ProcessResponseAsync(data, state);
        await ProcessLoginResult(loginResult);
    }

    private async Task ProcessLoginResult(LoginResult loginResult)
    {
        if (loginResult.IsError)
        {
            await _popupService.ShowPopup("Error", "Login process failed.");
            return;
        }

        var claims = loginResult.User.Claims.First().Subject!.Claims;
        var tokenModel = new TokenModel
        {
            UserName = claims.First(x => x.Type == "name").Value,
            UserId = claims.First(x => x.Type == "sub").Value,
            Role = claims.First(x => x.Type == "role").Value,
            AccessToken = loginResult.AccessToken,
            RefreshToken = loginResult.RefreshToken,
            AccessTokenExpiration = loginResult.AccessTokenExpiration
        };

        await _authStateProvider.MarkUserAsAuthenticated(tokenModel);

        _snackBar.Add("Login succeed.", Severity.Info);
    }

    public async Task LogoutAsync(bool forceLogout = false)
    {
        if (!forceLogout)
        {
            bool answer = await _popupService.ShowConfirm
                ("Logout", "Are you sure to logout?", okText: "Logout");
            if (!answer) return;
        }

        await _authStateProvider.MarkUserAsLoggedOut();
        _navigationManager.NavigateTo("");

        if (_oidcClient.Options.Browser != null)
        {
            await _oidcClient.LogoutAsync(new LogoutRequest());
        }
        else
        {
            string logoutUrl = await _oidcClient.PrepareLogoutAsync(new LogoutRequest());
            _navigationManager.NavigateTo(logoutUrl, forceLoad: true);
        }
    }

    public async Task<RefreshTokenResult?> RefreshTokenAsync()
    {
        var tokenModel = await _authStateProvider.GetTokenModelAsync();
        if (tokenModel == null) return null;

        var timeSpan = tokenModel.AccessTokenExpiration - DateTimeOffset.Now;
        if (timeSpan > RefreshTimeSpan * 2) return null;

        var result = await _oidcClient.RefreshTokenAsync(tokenModel.RefreshToken);
        if (result.IsError)
        {
            await LoginAsync();
            return await RefreshTokenAsync();
        }

        tokenModel.AccessToken = result.AccessToken;
        tokenModel.RefreshToken = result.RefreshToken;
        tokenModel.AccessTokenExpiration = result.AccessTokenExpiration;

        await _authStateProvider.SaveTokenModelAsync(tokenModel);

        return result;
    }
}
