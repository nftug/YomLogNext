using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive.Linq;

namespace YomLog.BlazorShared.Components;

public partial class AppBarContainer : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private HttpClientWrapper HttpClientWrapper { get; set; } = null!;

    [Parameter] public RenderFragment<AppBarContainer> ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment<AppBarContainer>? RightSection { get; set; }

    protected override void OnInitialized()
    {
        LayoutService.Page.Skip(1).Subscribe(_ => StateHasChanged());
        HttpClientWrapper.IsOffline.Skip(1).Subscribe(_ => StateHasChanged());
        LayoutService.AppBarRerenderRequested += StateHasChanged;
    }

    public async void BackButtonClicked()
    {
        string? backTo = NavigationManager.QueryString("BackTo");

        if (backTo != null)
            NavigationManager.NavigateTo(backTo);
        else
            await JSRuntime.InvokeVoidAsync("history.back", -1);
    }

    public void DrawerToggle()
    {
        LayoutService.DrawerOpen.Value = !LayoutService.DrawerOpen.Value;
    }
}
