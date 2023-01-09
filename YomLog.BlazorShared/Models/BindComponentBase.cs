using System.ComponentModel;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;

namespace YomLog.BlazorShared.Models;

public class BindComponentBase : ComponentBase, INotifyPropertyChanged, IDisposable
{
    private bool disposedValue;

    protected CompositeDisposable Disposable { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

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
}
