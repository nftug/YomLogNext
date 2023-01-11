// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE.MudBlazor file in the project root for more information.

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Models;
using YomLog.BlazorShared.Entities;
using YomLog.BlazorShared.Services.Repository;
using System.Reactive.Linq;

namespace YomLog.BlazorShared.Services;

public class LayoutService : BindableBase
{
    private readonly string Key = nameof(UserPreferences);

    private readonly IPreferenceRepositoryService _preference;

    public ReactivePropertySlim<UserPreferences> UserPreferences { get; } = null!;
    public ReactivePropertySlim<bool> DrawerOpen { get; }
    public ReactivePropertySlim<PageContainer?> Page { get; }
    public ReactivePropertySlim<Exception?> Exception = new();
    public ReactivePropertySlim<bool> IsInitializing { get; }

    public event Action? AppBarRerenderRequested;
    public void RequestAppBarRerender() => AppBarRerenderRequested?.Invoke();

    public LayoutService(IPreferenceRepositoryService preference)
    {
        _preference = preference;

        DrawerOpen = new ReactivePropertySlim<bool>().AddTo(Disposable);
        Page = new ReactivePropertySlim<PageContainer?>().AddTo(Disposable);

        UserPreferences = new ReactivePropertySlim<UserPreferences>().AddTo(Disposable);
        UserPreferences
            .Skip(1)
            .Subscribe(async v => await _preference.SaveAsync(Key, v));

        Exception = Exception.AddTo(Disposable);
        IsInitializing = new ReactivePropertySlim<bool>(true).AddTo(Disposable);
    }

    public bool IsDarkMode { get; private set; }

    public void SetDarkMode(bool value) => IsDarkMode = value;

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        var userPreferences = await _preference.GetAsync<UserPreferences>(Key) ?? new();
        if (userPreferences.DarkTheme != null)
        {
            IsDarkMode = (bool)userPreferences.DarkTheme;
            UserPreferences.Value = userPreferences;
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            UserPreferences.Value = new() { DarkTheme = IsDarkMode };
        }
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        UserPreferences.Value = new() { DarkTheme = IsDarkMode };
    }
}
