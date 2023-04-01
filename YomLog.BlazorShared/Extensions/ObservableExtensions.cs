using System.Reactive.Linq;

namespace YomLog.BlazorShared.Extensions;

public static class ObservableExtensions
{
    public static IObservable<TSource> ObserveOnMainThread<TSource>(this IObservable<TSource> source)
        => SynchronizationContext.Current != null
            ? source.ObserveOn(SynchronizationContext.Current)
            : source;

    public static IObservable<ObservableChangeRecord<TSource>> ObserveChanges<TSource>(this IObservable<TSource> source)
        => source.Zip(source.Skip(1), (p, c) => new ObservableChangeRecord<TSource>(p, c));
}

public record ObservableChangeRecord<T>(T Previous, T Current);