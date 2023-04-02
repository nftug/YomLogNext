using MudBlazor;

namespace YomLog.BlazorShared.Models;

public record AppSettings(
    bool IsNativeApp,
    Uri ApiBaseAddress,
    string AppName,
    MaxWidth DefaultMaxWidth = MaxWidth.Large
);
