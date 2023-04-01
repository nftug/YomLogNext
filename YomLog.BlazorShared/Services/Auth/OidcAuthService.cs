using Blazored.SessionStorage;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Popup;

namespace YomLog.BlazorShared.Services.Auth;

public class OidcAuthService : AuthServiceBase
{
    private readonly OidcClient _oidcClient;
    private readonly ISessionStorageService _sessionStorage;

    public OidcAuthService(
        AuthenticationStateProvider authStateProvider,
        IPopupService popupService,
        NavigationManager navigationManager,
        ISnackbar snackbar,
        OidcClient oidcClient,
        ISessionStorageService sessionStorage,
        ExceptionHubService exceptionHub

    ) : base(authStateProvider, popupService, navigationManager, snackbar, exceptionHub)
    {
        _oidcClient = oidcClient;
        _sessionStorage = sessionStorage;
    }

    public override async Task LoginAsync(string? redirectTo = null)
    {
        if (_oidcClient.Options.Browser != null)
        {
            var loginResult = await _oidcClient.LoginAsync(new LoginRequest());
            await ProcessLoginResult(loginResult, redirectTo);
        }
        else
        {
            var state = await _oidcClient.PrepareLoginAsync();
            await _sessionStorage.SetItemAsync("State", state);

            if (redirectTo != null)
                await _sessionStorage.SetItemAsStringAsync("RedirectTo", redirectTo);

            _navigationManager.NavigateTo(state.StartUrl, forceLoad: true);
        }
    }

    public override async Task ProcessLoginCallbackAsync(string data)
    {
        var state = await _sessionStorage.GetItemAsync<AuthorizeState>("State");
        var redirectTo = await _sessionStorage.GetItemAsStringAsync("RedirectTo");

        var loginResult = await _oidcClient.ProcessResponseAsync(data, state);
        await ProcessLoginResult(loginResult, redirectTo);
    }

    private async Task ProcessLoginResult(LoginResult loginResult, string? redirectTo)
    {
        if (loginResult.IsError)
        {
            await _popupService.ShowPopup("エラー", "ログインに失敗しました。");
            return;
        }

        var claims = loginResult.User.Claims;
        var tokenModel = new TokenModel
        {
            UserName = claims.First(x => x.Type == "name").Value,
            UserId = claims.First(x => x.Type == "sub").Value,
            AccessToken = loginResult.AccessToken,
            RefreshToken = loginResult.RefreshToken,
            AccessTokenExpiration = loginResult.AccessTokenExpiration
        };

        await LoginCoreAsync(tokenModel, redirectTo);
    }

    protected override async Task OnAfterLogoutAsync()
    {
        // For Auth0, you must change logout endpoint URL.
        // https://auth0.com/docs/api/authentication#logout

        var logoutUrl = QueryHelpers.AddQueryString(
            $"{_oidcClient.Options.Authority}/v2/logout",
            new Dictionary<string, string>
            {
                { "client_id", _oidcClient.Options.ClientId },
                { "returnTo", _oidcClient.Options.PostLogoutRedirectUri }
            });

        if (_oidcClient.Options.Browser != null)
        {
            // await _oidcClient.LogoutAsync();
            await _oidcClient.Options.Browser.InvokeAsync(new(logoutUrl, _oidcClient.Options.PostLogoutRedirectUri));
            _navigationManager.NavigateTo("");
            _snackbar.Add("ログアウトしました。", Severity.Info);
        }
        else
        {
            // string logoutUrl = await _oidcClient.PrepareLogoutAsync(new LogoutRequest());
            _navigationManager.NavigateTo(logoutUrl, forceLoad: true);
        }
    }

    public override async Task RefreshTokenAsync()
    {
        var tokenModel = await _authStateProvider.GetTokenModelAsync();
        if (tokenModel == null) return;

        var timeSpan = tokenModel.AccessTokenExpiration - DateTimeOffset.Now;
        if (timeSpan > RefreshTimeSpan * 2)
        {
            IsTokenValid.Value = true;
            return;
        }

        var result = await _oidcClient.RefreshTokenAsync(tokenModel.RefreshToken, scope: "offline_access");
        if (result.IsError)
        {
            await LoginAsync(_navigationManager.ToBaseRelativePath(_navigationManager.Uri));
            await RefreshTokenAsync();
        }

        tokenModel.AccessToken = result.AccessToken;
        tokenModel.RefreshToken = result.RefreshToken;
        tokenModel.AccessTokenExpiration = result.AccessTokenExpiration;

        IsTokenValid.Value = true;

        await _authStateProvider.SaveTokenModelAsync(tokenModel);
    }
}
