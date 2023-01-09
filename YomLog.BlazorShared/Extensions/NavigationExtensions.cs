using System.Collections.Specialized;
using System.Web;
using YomLog.BlazorShared.Models;
using Microsoft.AspNetCore.Components;

namespace YomLog.BlazorShared.Extensions;

public static class NavigationExtensions
{
    // get entire querystring name/value collection
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    // get single querystring value with specified key
    public static string? QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager?.QueryString()[key];
    }

    public static bool CheckMatchRelativePath(
        this NavigationManager navigationManager,
        string relativePath,
        NavPathLinkMatch match = NavPathLinkMatch.WithoutQuery
    )
    {
        var currentUri = new Uri(navigationManager.Uri);
        string currentPathToMatch = match == NavPathLinkMatch.All ? currentUri.PathAndQuery : currentUri.LocalPath;
        if (relativePath.FirstOrDefault() != '/') relativePath = "/" + relativePath;

        return match == NavPathLinkMatch.Prefix
            ? currentPathToMatch.StartsWith(relativePath)
            : currentPathToMatch == relativePath;
    }
}