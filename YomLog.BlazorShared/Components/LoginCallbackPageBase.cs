using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Services.Auth;

namespace YomLog.BlazorShared.Components;

[AllowAnonymous]
public abstract class LoginCallbackPageBase : ComponentBase
{
    [Inject] private PageInfoService PageInfoService { get; set; } = null!;
    [Inject] private IAuthService AuthService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await AuthService.ProcessLoginCallbackAsync(PageInfoService.Query.Value);
    }
}
