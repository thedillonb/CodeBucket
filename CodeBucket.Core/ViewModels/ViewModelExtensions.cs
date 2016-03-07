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

namespace CodeBucket.Core.ViewModels
{
    public static class ViewModelExtensions
    {
        public static async Task RequestModel<TRequest>(this MvxViewModel viewModel, Func<TRequest> request, Action<TRequest> update) where TRequest : new()
        {
            var data = await Task.Run(() => request());
            update(data);
        }

		public static void CreateMore<T>(this MvxViewModel viewModel, BitbucketSharp.Models.V2.Collection<T> response, 
										 Action<Action> assignMore, Action<IEnumerable<T>> newDataAction)
        {
			if (string.IsNullOrEmpty(response.Next))
            {
                assignMore(null);
                return;
            }

			Action task = () => 
			{
				var moreResponse = Mvx.Resolve<IApplicationService>().Client.Request2<BitbucketSharp.Models.V2.Collection<T>>(response.Next);
                viewModel.CreateMore(moreResponse, assignMore, newDataAction);
				newDataAction(moreResponse.Values);
        	};

			assignMore(task);
        }

		public static Task SimpleCollectionLoad<T>(this CollectionViewModel<T> viewModel, Func<List<T>> request)
        {
			return viewModel.RequestModel(request, response => {
				//viewModel.CreateMore(response, m => viewModel.MoreItems = m, viewModel.Items.AddRange);
                viewModel.Items.Reset(response);
            });
        }

		public static Task SimpleCollectionLoad<T>(this CollectionViewModel<T> viewModel, Func<BitbucketSharp.Models.V2.Collection<T>> request)
		{
            var weakVm = new WeakReference<CollectionViewModel<T>>(viewModel);
            return viewModel.RequestModel(request, response =>
            {
                weakVm.Get()?.CreateMore(response, m => {
                    var weak = weakVm.Get();
                    if (weak != null)
                        weak.MoreItems = m;
                }, viewModel.Items.AddRange);
                weakVm.Get()?.Items.Reset(response.Values);
            });
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

