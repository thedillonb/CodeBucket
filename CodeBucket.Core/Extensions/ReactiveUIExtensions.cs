namespace ReactiveUI
{
    using System;
    using System.Reactive.Disposables;

    public static class WhenActivatedExtensions
    {
        public static IDisposable WhenActivated(
            this IActivatable @this,
            Action<CompositeDisposable> disposables,
            IViewFor view = null)
        {
            return @this.WhenActivated(() =>
            {
                var d = new CompositeDisposable();
                disposables(d);
                return new[] { d };
            },
            view);
        }
    }
}