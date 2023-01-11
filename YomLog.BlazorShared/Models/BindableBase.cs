using System.ComponentModel;
using System.Reactive.Disposables;

namespace YomLog.BlazorShared.Models;

public abstract class BindableBase : INotifyPropertyChanged, IDisposable
{
    private bool disposedValue;

    protected CompositeDisposable Disposable { get; } = new();

#pragma warning disable CS0067
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
