using System.Diagnostics;

namespace YomLog.BlazorShared.Services;

public class DebugLoggerService : IDebugLoggerService
{
    public void Print<T>(string message) => Debug.WriteLine($"{typeof(T).Name}: {message}");
    public void Print(string message) => Debug.WriteLine(message);
}