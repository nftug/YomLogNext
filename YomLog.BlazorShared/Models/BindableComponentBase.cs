using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;

namespace YomLog.BlazorShared.Models;

public class BindableComponentBase : ComponentBase, IDisposable
{
    private bool disposedValue;

    protected CompositeDisposable Disposable { get; } = new();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Disposable.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected void Rerender() => InvokeAsync(StateHasChanged);
}
