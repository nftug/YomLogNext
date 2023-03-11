// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE.MudBlazor file in the project root for more information.

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Services.Repository;

namespace YomLog.BlazorShared.Services;

public class LayoutService : BindableBase
{
    private readonly string Key = nameof(UserPreferences);

    private readonly IPreferenceRepositoryService _preference;

    public ReactivePropertySlim<UserPreferences> UserPreferences { get; }
    public ReactivePropertySlim<bool> DrawerOpen { get; }
    public ReactivePropertySlim<PageContainer?> Page { get; }
    public ReactivePropertySlim<bool> IsProcessing { get; }
    public ReactivePropertySlim<bool> IsDarkMode { get; }

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
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        UserPreferences.Value = new() { DarkTheme = isDarkModeDefaultTheme };
        IsDarkMode.Value = isDarkModeDefaultTheme;

        var userPreferences = await _preference.GetAsync<UserPreferences>(Key) ?? new();
        if (userPreferences.DarkTheme != null)
        {
            IsDarkMode.Value = (bool)userPreferences.DarkTheme;
            userPreferences.DarkTheme = IsDarkMode.Value;
        }

        UserPreferences.Value = userPreferences;
    }

    public async Task ToggleDarkMode()
    {
        IsDarkMode.Value = !IsDarkMode.Value;
        UserPreferences.Value.DarkTheme = IsDarkMode.Value;
        await _preference.SaveAsync(Key, UserPreferences.Value);
    }
}
