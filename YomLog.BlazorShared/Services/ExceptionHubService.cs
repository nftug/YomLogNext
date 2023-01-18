using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;

namespace YomLog.BlazorShared.Services;

public class ExceptionHubService : BindableBase
{
    public ReactivePropertySlim<Exception?> Exception { get; }

    public ExceptionHubService()
    {
        Exception = new ReactivePropertySlim<Exception?>().AddTo(Disposable);
    }

    public async Task Handle(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            Exception.Value = e;
        }
    }
}
