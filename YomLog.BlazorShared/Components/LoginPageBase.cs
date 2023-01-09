using YomLog.BlazorShared.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using YomLog.BlazorShared.Models;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace YomLog.BlazorShared.Components;

[AllowAnonymous]
public abstract class LoginPageBase : BindComponentBase
{
    [Inject] protected OidcAuthService AuthService { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string? Redirect { get; set; }

    protected override void OnInitialized()
    {
        AuthService.IsAuthenticated.Where(v => v).Subscribe(_ => OnLogin()).AddTo(Disposable);
    }

    private bool _preventDispose;

    protected override void Dispose(bool disposing)
    {
        if (_preventDispose) return;
        base.Dispose(disposing);
    }

    protected async Task Login()
    {
        // When authentication state changed on WASM client, for some reason component is disposed.
        // So it needs to prevent disposing automatically.
        _preventDispose = true;
        await AuthService.LoginAsync();
    }

    protected virtual void OnLogin()
    {
        _preventDispose = false;
        Dispose();
        NavigationManager.NavigateTo(Redirect ?? "", replace: true);
    }
}
