using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using YomLog.BlazorShared.Models;

namespace YomLog.BlazorShared.Services;

public class PageInfoService : BindableBase
{
    private readonly NavigationManager _navigationManager;

    public PageInfoService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += SetCurrentPath;
    }

    protected override void Dispose(bool disposing)
    {
        _navigationManager.LocationChanged -= SetCurrentPath;
    }

    public event Action<string, string>? PathChanged;
    public event Action? QueryParameterChanged;

    public string CurrentPath => new Uri(_navigationManager.Uri).PathAndQuery;
    public string PreviousPath { get; private set; } = string.Empty;

    public string CurrentRoute => CurrentPath.Split('?').First();
    public string PreviousRoute => PreviousPath.Split('?').First();
    public string? CurrentQuery => CurrentPath.Split('?').Skip(1).FirstOrDefault();
    public string? PreviousQuery => PreviousPath.Split('?').Skip(1).FirstOrDefault();
    public bool IsQueryUpdated => CurrentRoute == PreviousRoute && CurrentQuery != PreviousQuery;

    public void SetCurrentPath(object? sender, LocationChangedEventArgs e)
    {
        if (PreviousPath != CurrentPath)
            PathChanged?.Invoke(CurrentPath, PreviousPath);

        PreviousPath = CurrentPath;

        if (IsQueryUpdated)
            QueryParameterChanged?.Invoke();
    }
}