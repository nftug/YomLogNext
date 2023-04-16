using System.Reactive.Linq;
using MudBlazor;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using YomLog.BlazorShared.Components;
using YomLog.BlazorShared.Extensions;
using YomLog.BlazorShared.Models;
using YomLog.Shared.Attributes;
using YomLog.Shared.Exceptions;

namespace YomLog.BlazorShared.Services;

[InjectAsScoped]
public class ExceptionHubService : BindableBase
{
    protected readonly LayoutService _layoutService;
    protected readonly IDialogService _dialogService;

    public ReactivePropertySlim<Exception?> Exception { get; }

    public ExceptionHubService(LayoutService layoutService, IDialogService dialogService)
    {
        _layoutService = layoutService;
        _dialogService = dialogService;

        Exception = new ReactivePropertySlim<Exception?>().AddTo(Disposable);

        Exception
            .ObserveOnMainThread()
            .Where(v => v != null)
            .Subscribe(async v =>
            {
                _layoutService.IsProcessing.Value = false;
                Exception.Value = null;

                // If switched to offline mode, do nothing.
                if (v is IApiException ex && ex.StatusCode is null) return;

                await ExceptionDialog.ShowDialog(_dialogService, v!);
            });
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
