// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE.MudBlazor file in the project root for more information.

using MudBlazor;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Repository;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
public class LayoutService : BindableBase
{
    private readonly string Key = nameof(UserPreferences);

    private readonly IPreferenceRepositoryService _preference;

    public ReactivePropertySlim<UserPreferences> UserPreferences { get; }
    public ReactivePropertySlim<bool> DrawerOpen { get; }
    public ReactivePropertySlim<PageContainer?> Page { get; }
    public ReactivePropertySlim<bool> IsProcessing { get; }
    public ReactivePropertySlim<bool> IsDarkMode { get; }
    public ReactivePropertySlim<bool> ShowingDialog { get; }

    public event Action? AppBarRerenderRequested;
    public void RequestAppBarRerender() => AppBarRerenderRequested?.Invoke();

    public LayoutService(IPreferenceRepositoryService preference)
    {
        _preference = preference;

        DrawerOpen = new ReactivePropertySlim<bool>().AddTo(Disposable);
        Page = new ReactivePropertySlim<PageContainer?>().AddTo(Disposable);
        UserPreferences = new ReactivePropertySlim<UserPreferences>().AddTo(Disposable);
        IsProcessing = new ReactivePropertySlim<bool>().AddTo(Disposable);
        IsDarkMode = new ReactivePropertySlim<bool>().AddTo(Disposable);
        ShowingDialog = new ReactivePropertySlim<bool>().AddTo(Disposable);
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        UserPreferences.Value = new(DarkTheme: isDarkModeDefaultTheme);
        IsDarkMode.Value = isDarkModeDefaultTheme;

        UserPreferences.Value =
            await _preference.GetAsync<UserPreferences>(Key) ?? new(DarkTheme: isDarkModeDefaultTheme);

        if (UserPreferences.Value.DarkTheme is bool darkTheme)
            IsDarkMode.Value = darkTheme;
    }

    public async Task ToggleDarkMode()
    {
        IsDarkMode.Value = !IsDarkMode.Value;
        UserPreferences.Value = UserPreferences.Value with { DarkTheme = IsDarkMode.Value };
        await _preference.SaveAsync(Key, UserPreferences.Value);
    }

    public void OnSwipe(SwipeDirection direction)
    {
        if (ShowingDialog.Value) return;

        if (direction == SwipeDirection.LeftToRight && !DrawerOpen.Value)
            DrawerOpen.Value = true;
        else if (direction == SwipeDirection.RightToLeft && DrawerOpen.Value)
            DrawerOpen.Value = false;
    }
}
