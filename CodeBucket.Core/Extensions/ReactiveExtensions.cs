// ReSharper disable once CheckNamespace
using ReactiveUI;
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

        public static IDisposable SubscribeSafe<T>(this IObservable<T> obs, Action<T> act)
        {
            return obs.Subscribe(x =>
            {
                try
                {
                    act(x);
                }
                catch (Exception e)
                {
                    RxApp.DefaultExceptionHandler.OnNext(e);
                }
            }, RxApp.DefaultExceptionHandler.OnNext);
        }
    }
}

