
// Analysis disable once CheckNamespace
namespace System.Reactive.Linq
{
    using System;

    public static class ObservableExtensions
    {
        public static IObservable<T> IsNotNull<T>(this IObservable<T> @this) where T : class
        {
            return @this.Where(x => x != null);
        }
    }
}

// Analysis disable once CheckNamespace
namespace System
{
    using Windows.Input;
    using Reactive.Disposables;
    using Reactive.Linq;

    public static class ObservableExtensions
    {
        public static IDisposable BindCommand<T>(this IObservable<T> @this, ICommand command)
        {
            return command == null ? Disposable.Empty : @this.Where(x => command.CanExecute(x)).Subscribe(x => command.Execute(x));
        }
    }
}
