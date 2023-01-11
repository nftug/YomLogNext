using IdentityModel.OidcClient;
using MudBlazor;

namespace YomLog.BlazorShared.Models;

public class AppSettings
{
    public bool IsNativeApp { get; init; }
    public MaxWidth DefaultMaxWidth { get; init; } = MaxWidth.Large;
    public Uri ApiBaseAddress { get; set; } = null!;
    public string AppName { get; init; } = "My Blazor App";
    public OidcClientOptions OidcClientOptions { get; set; } = null!;
}
