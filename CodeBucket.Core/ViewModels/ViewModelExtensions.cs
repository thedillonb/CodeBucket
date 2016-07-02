using System;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels
{
    public static class ViewModelExtensions
    {
		public static void CreateMore<T>(this MvxViewModel viewModel, Collection<T> response, 
										 Action<Func<Task>> assignMore, Action<IEnumerable<T>> newDataAction)
        {
			if (string.IsNullOrEmpty(response.Next))
            {
                assignMore(null);
                return;
            }

            assignMore(new Func<Task>(async () =>
            {
                var moreResponse = await Mvx.Resolve<IApplicationService>().Client.Get<Collection<T>>(response.Next);
                viewModel.CreateMore(moreResponse, assignMore, newDataAction);
                newDataAction(moreResponse.Values);
            }));
        }

		public static async Task SimpleCollectionLoad<T>(
            this CollectionViewModel<T> viewModel, Func<Task<Collection<T>>> request)
		{
            var weakVm = new WeakReference<CollectionViewModel<T>>(viewModel);
            var response = await request();
            weakVm.Get()?.CreateMore(response, m =>
            {
                var weak = weakVm.Get();
                if (weak != null)
                    weak.MoreItems = m;
            }, viewModel.Items.AddRange);
            weakVm.Get()?.Items.Reset(response.Values);
		}
    }
}

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
        return activate ? ret.StartWith(comp(viewModel)) : ret;
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

