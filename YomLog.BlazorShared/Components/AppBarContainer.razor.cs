using YomLog.BlazorShared.Services;
using YomLog.BlazorShared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive.Linq;
using YomLog.BlazorShared.Models;
using Reactive.Bindings.Extensions;
using Microsoft.AspNetCore.Components.Routing;
using System.Reactive.Concurrency;

namespace YomLog.BlazorShared.Components;

public partial class AppBarContainer : BindableComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private HttpClientWrapper HttpClientWrapper { get; set; } = null!;

    [Parameter] public RenderFragment<AppBarContainer> ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment<AppBarContainer>? RightSection { get; set; }

    protected override void OnInitialized()
    {
        LayoutService.Page.
            Skip(1)
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);
        HttpClientWrapper.IsOffline
            .Skip(1)
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);

        LayoutService.AppBarRerenderRequested += Rerender;
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    protected override void Dispose(bool disposing)
    {
        LayoutService.AppBarRerenderRequested -= Rerender;
        NavigationManager.LocationChanged -= OnLocationChanged;
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

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => Rerender();
}
