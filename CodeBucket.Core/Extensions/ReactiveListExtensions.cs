using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

// ReSharper disable once CheckNamespace
namespace System
{
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