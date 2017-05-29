using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

// ReSharper disable once CheckNamespace
namespace ReactiveUI
{
    public static class ReactiveCommandFactory
    {
        public static ReactiveCommand<Unit, Unit> Empty()
        {
            return ReactiveCommand.Create(() => { });
        }
    }

    public static class ObservableExtensions
    {
        public static IDisposable BindCommand<TIn, TOut>(this IObservable<TIn> This, ReactiveCommand<TIn, TOut> cmd)
        {
            return This.InvokeCommand(cmd);
        }

        public static IDisposable BindCommand<T, TResult, TTarget>(this IObservable<T> This, TTarget target, Expression<Func<TTarget, ReactiveCommandBase<T, TResult>>> commandProperty)
        {
            return This.InvokeCommand(target, commandProperty);
        }
    }

    public static class ReactiveListExtensions
    {
        public static void Reset<T>(this IReactiveList<T> @this, IEnumerable<T> items)
        {
            using (@this.SuppressChangeNotifications())
            {
                @this.Clear();
                @this.AddRange(items);
            }
        }

        public static IObservable<IReadOnlyReactiveList<T>> ChangedObservable<T>(this IReadOnlyReactiveList<T> @this)
        {
            return @this.Changed
                        .Select(_ => Unit.Default)
                        .StartWith(Unit.Default)
                        .Select(_ => @this);
        }
    }
}