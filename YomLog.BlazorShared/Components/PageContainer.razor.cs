using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace YomLog.BlazorShared.Components;

public partial class PageContainer : BindComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private HttpClientWrapper HttpClientWrapper { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private IEnvironmentHelper EnvironmentHelper { get; set; } = null!;
    [Inject] private AppSettings AppSettings { get; set; } = null!;

    [Parameter] public string? Title { get; set; }
    [Parameter] public AppBarLeftButton LeftButton { get; set; } = AppBarLeftButton.Drawer;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment? MainContent { get; set; }
    [Parameter] public RenderFragment<AppBarContainer>? TitleContent { get; set; }
    [Parameter] public RenderFragment<AppBarContainer>? AppBarActions { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }
    [Parameter] public string Class { get; set; } = "mb-8 mt-8";
    [Parameter] public MaxWidth? MaxWidth { get; set; }
    [Parameter] public bool CenteredContainer { get; set; }
    [Parameter] public bool TopPage { get; set; }
    [Parameter] public bool SetHeader { get; set; }

    private MaxWidth ContainerMaxWidth => MaxWidth ?? AppSettings.DefaultMaxWidth;

    public string MainContentClass
        => FooterContent != null ? "mud-main-content-with-footer" : string.Empty;

    public string OuterContainerClass
        => CenteredContainer
            ? $"outer-centered-container {Class}"
            : $"{(HttpClientWrapper.IsOffline.Value ? "mt-10 " : null)}{Class}";

    protected override void OnInitialized()
    {
        LayoutService.Page.Value = this;
        HttpClientWrapper.IsOffline.Skip(1).Subscribe(_ => StateHasChanged()).AddTo(Disposable);
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;
        if (TopPage && AppSettings.IsNativeApp)
            await JSRuntime.InvokeVoidAsync("onRenderTopPage", DotNetObjectReference.Create(this));
    }

    protected override async void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (TopPage && AppSettings.IsNativeApp)
            await JSRuntime.InvokeVoidAsync("onLeaveTopPage");

        base.Dispose(disposing);
    }

    [JSInvokable("GoBackOnTopPage")]
    public void OnGoBackOnTopPage() => EnvironmentHelper.QuitApp();
}
