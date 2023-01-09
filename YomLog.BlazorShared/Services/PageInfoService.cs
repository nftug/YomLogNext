using Microsoft.AspNetCore.Components;

namespace YomLog.BlazorShared.Services;

public class PageInfoService
{
    private readonly NavigationManager _navigationManager;

    public PageInfoService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += (_, _) => SetCurrentPath();
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

    public void SetCurrentPath()
    {
        PreviousPath = CurrentPath;

        PathChanged?.Invoke(CurrentPath, PreviousPath);

        if (IsQueryUpdated)
            QueryParameterChanged?.Invoke();
    }
}