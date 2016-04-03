using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;

public static class BindExtensions
{
    public static IObservable<TR> Bind<T, TR>(this T viewModel, System.Linq.Expressions.Expression<Func<T, TR>> outExpr, bool activate = false) where T : INotifyPropertyChanged
    {
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        var comp = outExpr.Compile();

        var ret = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(t => viewModel.PropertyChanged += t, t => viewModel.PropertyChanged -= t)
            .Where(x => string.Equals(x.EventArgs.PropertyName, name))
            .Select(x => comp(viewModel));

        if (!activate)
            return ret;

        var o = Observable.Create<TR>(obs => {
            try
            {
                obs.OnNext(comp(viewModel));
            }
            catch (Exception e)
            {
                obs.OnError(e);
            }

            obs.OnCompleted();

            return Disposable.Empty;
        });

        return o.Concat(ret);
    }

    public static IObservable<Unit> BindCollection<T>(this T viewModel, System.Linq.Expressions.Expression<Func<T, INotifyCollectionChanged>> outExpr, bool activate = false) where T : INotifyPropertyChanged
    {
        var exp = outExpr.Compile();
        var m = exp(viewModel);

        var ret = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(t => m.CollectionChanged += t, t => m.CollectionChanged -= t)
            .Select(_ => Unit.Default);
        return activate ? ret.StartWith(Unit.Default) : ret;
    }
}

