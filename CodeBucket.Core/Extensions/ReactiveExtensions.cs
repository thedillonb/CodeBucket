// ReSharper disable once CheckNamespace
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace System
{
    public static class ReactiveExtensions
    {
        public static void AddTo(this IDisposable @this, CompositeDisposable disposable)
            => disposable.Add(@this);

        public static IObservable<Unit> SelectUnit<T>(this IObservable<T> obs)
            => obs.Select(x => Unit.Default);
    }
}

