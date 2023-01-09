using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using Reactive.Bindings;
using YomLog.BlazorShared.Services.Repository;

namespace YomLog.BlazorShared.Services.Auth;

public class MyAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IPreferenceRepositoryService _preferenceRepository;

    private readonly string Key = "LoginResult";

    public ReactivePropertySlim<ClaimsIdentity> Identity { get; } = new();

    public MyAuthenticationStateProvider
        (HttpClient httpClient, IPreferenceRepositoryService preferenceRepository)
    {
        _httpClient = httpClient;
        _preferenceRepository = preferenceRepository;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var tokenModel = await GetTokenModelAsync();

        if (tokenModel == null)
        {
            Identity.Value = new ClaimsIdentity();
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", tokenModel.AccessToken);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, tokenModel.UserName),
                new Claim(ClaimTypes.NameIdentifier, tokenModel.UserId),
                new Claim(ClaimTypes.Role, tokenModel.Role)
            };

            Identity.Value = new ClaimsIdentity(claims, "User");
        }

        return new AuthenticationState(new ClaimsPrincipal(Identity.Value));
    }

    public async Task MarkUserAsAuthenticated(TokenModel tokenModel)
    {
        await SaveTokenModelAsync(tokenModel);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _preferenceRepository.RemoveAsync(Key);
        _httpClient.DefaultRequestHeaders.Clear();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<TokenModel?> GetTokenModelAsync()
        => await _preferenceRepository.GetAsync<TokenModel>(Key);

    public async Task SaveTokenModelAsync(TokenModel tokenModel)
        => await _preferenceRepository.SaveAsync(Key, tokenModel);
}
