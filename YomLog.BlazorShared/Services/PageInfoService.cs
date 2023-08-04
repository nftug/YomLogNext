using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
public class PageInfoService : BindableBase
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    private ReactivePropertySlim<Uri> Uri { get; }

    public ReadOnlyReactivePropertySlim<string> PathAndQuery { get; }
    public ReadOnlyReactivePropertySlim<string> LocalPath { get; }
    public ReadOnlyReactivePropertySlim<string> Query { get; }
    public ReactivePropertySlim<bool> PopStateInvoked { get; }

    public PageInfoService(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;

        Uri = new ReactivePropertySlim<Uri>(new(navigationManager.Uri));
        PathAndQuery = Uri
            .Select(x => x.PathAndQuery)
            .ToReadOnlyReactivePropertySlim<string>();
        LocalPath = Uri
            .Select(x => x.LocalPath)
            .ToReadOnlyReactivePropertySlim<string>();
        Query = Uri
            .Select(x => x.Query)
            .ToReadOnlyReactivePropertySlim<string>();
        PopStateInvoked = new ReactivePropertySlim<bool>();

        // NOTE: For debug
        // PathAndQuery.ObserveChanges().Subscribe(x => System.Diagnostics.Debug.WriteLine(x));

        _navigationManager.LocationChanged += (_, e) => Uri.Value = new(e.Location);
        Task.Run(() => _jsRuntime.InvokeVoidAsync("registerPageInfoService", DotNetObjectReference.Create(this)));
    }

    [JSInvokable("OnPopState")]
    public async Task OnPopState()
    {
        System.Diagnostics.Debug.WriteLine("OnPopState");
        PopStateInvoked.Value = true;
        await Task.Delay(100);
        PopStateInvoked.Value = false;
    }
}
