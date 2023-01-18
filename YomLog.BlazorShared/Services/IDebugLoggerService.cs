namespace YomLog.BlazorShared.Services;

public interface IDebugLoggerService
{
    void Print<T>(string message);
    void Print(string message);
}
