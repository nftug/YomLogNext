using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Shared.Attributes;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
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
