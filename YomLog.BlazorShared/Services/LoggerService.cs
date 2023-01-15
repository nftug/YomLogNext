using Microsoft.JSInterop;

namespace YomLog.BlazorShared.Services;

public class LoggerService
{
    private readonly IJSRuntime _jsRuntime;

    public LoggerService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public virtual async Task Print<T>(string message)
    {
        await _jsRuntime.InvokeVoidAsync("console.log", $"Log from {typeof(T).Name}:");
        await Print(message);
    }

    public virtual async Task Print(string message)
    {
        await _jsRuntime.InvokeVoidAsync("console.log", message);
    }
}
