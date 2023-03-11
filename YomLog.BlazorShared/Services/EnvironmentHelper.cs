namespace YomLog.BlazorShared.Services;

public class EnvironmentHelper : IEnvironmentHelper
{
    public virtual void QuitApp() { }
}

public interface IEnvironmentHelper
{
    void QuitApp();
}
