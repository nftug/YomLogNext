﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using YomLog.BlazorShared.Services;
using MudBlazor;
using System.Reactive.Linq;
using YomLog.BlazorShared.Models;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;

namespace YomLog.BlazorShared.Components;

public partial class MainLayoutContainer : BindableComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private ScrollInfoService ScrollInfoService { get; set; } = null!;

    [Parameter] public bool? IsDarkModeDefault { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public RenderFragment? DrawerSection { get; set; }
    [Parameter] public RenderFragment AppBarSection { get; set; } = null!;
    [Parameter] public RenderFragment? NotFoundSection { get; set; }

    private ErrorBoundary? _errorBoundary;
    private MudThemeProvider? _mudThemeProvider;

    protected override async Task OnInitializedAsync()
    {
        LayoutService.UserPreferences
            .ObserveOn(SynchronizationContext.Current!)
            .Skip(1)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);

        LayoutService.IsDarkMode
            .ObserveOn(SynchronizationContext.Current!)
            .Skip(1)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);

        LayoutService.Page
            .ObserveOn(SynchronizationContext.Current!)
            .Where(v => v != null)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);

        LayoutService.IsProcessing
            .ObserveOn(SynchronizationContext.Current!)
            .Skip(1)
            .Subscribe(_ => Rerender())
            .AddTo(Disposable);

        await ScrollInfoService.RegisterService();
        await ApplyUserPreferences();
    }

    protected override void OnParametersSet()
    {
        _errorBoundary?.Recover();
    }

    private async Task ApplyUserPreferences()
    {
        var defaultDarkMode = IsDarkModeDefault ?? await _mudThemeProvider!.GetSystemPreference();
        await LayoutService.ApplyUserPreferences(defaultDarkMode);
    }
}