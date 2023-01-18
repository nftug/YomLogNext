using System.Security.Principal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace YomLog.BlazorShared.Components;

public abstract class AuthorizeComponentBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected IIdentity? CurrentUser;
    protected bool IsAuthenticated => CurrentUser?.IsAuthenticated == true;

    protected async override Task OnParametersSetAsync()
    {
        if (AuthenticationStateTask == null) return;
        CurrentUser = (await AuthenticationStateTask).User.Identity;
    }
}
