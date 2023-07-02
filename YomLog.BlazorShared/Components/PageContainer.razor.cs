using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using YomLog.BlazorShared.Enums;
using Microsoft.AspNetCore.Components.Routing;

namespace YomLog.BlazorShared.Components;

public partial class PageContainer : BindableComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private HttpClientWrapper HttpClientWrapper { get; set; } = null!;
    [Inject] private AppSettings AppSettings { get; set; } = null!;
    [Inject] private PageInfoService PageInfoService { get; set; } = null!;
    [Inject] private IEnvironmentHelper EnvironmentHelper { get; set; } = null!;

    [Parameter] public string? Title { get; set; }
    [Parameter] public AppBarLeftButton LeftButton { get; set; } = AppBarLeftButton.Drawer;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment? MainContent { get; set; }
    [Parameter] public RenderFragment<AppBarContainer>? TitleContent { get; set; }
    [Parameter] public RenderFragment<AppBarContainer>? AppBarActions { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }
    [Parameter] public string Class { get; set; } = "my-8";
    [Parameter] public MaxWidth? MaxWidth { get; set; }
    [Parameter] public bool CenteredContainer { get; set; }
    [Parameter] public bool TopPage { get; set; }
    [Parameter] public bool SetHeader { get; set; }

    private MaxWidth ContainerMaxWidth => MaxWidth ?? AppSettings.DefaultMaxWidth;

    public string MainContentClass
        => FooterContent != null ? "mud-main-content-with-footer" : string.Empty;

    private string OuterContainerClass
        => CenteredContainer
            ? $"outer-centered-container"
            : $"{(HttpClientWrapper.IsOffline.Value ? "mt-10 " : null)}{Class}";

    protected override void OnInitialized()
    {
        LayoutService.Page.Value = this;
        HttpClientWrapper.IsOffline.Skip(1).Subscribe(_ => Rerender()).AddTo(Disposable);
    }

    private async Task OnBeforeNavigationOnTopPage(LocationChangingContext context)
    {
        if (!PageInfoService.PopStateInvoked.Value || LayoutService.ShowingDialog.Value) return;
        context.PreventNavigation();
        await EnvironmentHelper.QuitApp();
    }
}
