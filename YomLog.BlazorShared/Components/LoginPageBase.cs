using YomLog.BlazorShared.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using YomLog.BlazorShared.Models;

namespace YomLog.BlazorShared.Components;

[AllowAnonymous]
public abstract class LoginPageBase : BindableComponentBase
{
    [Inject] protected IAuthService AuthService { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string? Redirect { get; set; }

    protected async Task Login() => await AuthService.LoginAsync(Redirect ?? "");
}
