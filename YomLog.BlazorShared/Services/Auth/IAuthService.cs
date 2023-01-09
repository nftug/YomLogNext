using Reactive.Bindings;

namespace YomLog.BlazorShared.Services.Auth;

public interface IAuthService
{
    ReactiveTimer RefreshTimer { get; }
    ReadOnlyReactivePropertySlim<bool> IsAuthenticated { get; }

    Task LoginAsync(string? redirectTo = null);
    Task ProcessLoginCallbackAsync(string data);
    Task LogoutAsync(bool forceLogout = false);
    Task RefreshTokenAsync();
}
