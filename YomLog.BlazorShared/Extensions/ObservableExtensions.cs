using System.Reactive.Linq;

namespace YomLog.BlazorShared.Extensions;

public static class ObservableExtensions
{
    public static IObservable<TSource> ObserveOnMainThread<TSource>(this IObservable<TSource> source)
        => SynchronizationContext.Current != null
            ? source.ObserveOn(SynchronizationContext.Current)
            : source;
}