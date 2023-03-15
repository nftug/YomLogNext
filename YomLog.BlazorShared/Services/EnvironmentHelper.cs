using YomLog.Shared.Extensions;

namespace YomLog.BlazorShared.Services;

[InjectAsTransient]
public class EnvironmentHelper : IEnvironmentHelper
{
    public virtual Task QuitApp() => Task.FromResult(0);
}

public interface IEnvironmentHelper
{
    Task QuitApp();
}
