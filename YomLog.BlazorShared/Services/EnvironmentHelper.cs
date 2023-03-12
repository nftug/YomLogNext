namespace YomLog.BlazorShared.Services;

public class EnvironmentHelper : IEnvironmentHelper
{
    public virtual Task QuitApp() => Task.FromResult(0);
}

public interface IEnvironmentHelper
{
    Task QuitApp();
}
