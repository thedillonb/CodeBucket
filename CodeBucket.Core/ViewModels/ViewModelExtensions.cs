using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

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
			return viewModel.RequestModel(request, response => {
				viewModel.CreateMore(response, m => viewModel.MoreItems = m, viewModel.Items.AddRange);
				viewModel.Items.Reset(response.Values);
			});
		}
    }
}

public static class BindExtensions
{
    public static void Bind<T, TR>(this T viewModel, System.Linq.Expressions.Expression<Func<T, TR>> outExpr, Action b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName.Equals(name))
            {
                try
                {
                    b();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        };

        if (activateNow)
        {
            try
            {
                b();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public static void Bind<T, TR>(this T viewModel, System.Linq.Expressions.Expression<Func<T, TR>> outExpr, Action<TR> b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        var comp = outExpr.Compile();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName.Equals(name))
            {
                try
                {
                    b(comp(viewModel));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        };

        if (activateNow)
        {
            try
            {
                b(comp(viewModel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public static void BindCollection<T>(this T viewModel, System.Linq.Expressions.Expression<Func<T, INotifyCollectionChanged>> outExpr, Action<NotifyCollectionChangedEventArgs> b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var exp = outExpr.Compile();
        var m = exp(viewModel);
        m.CollectionChanged += (sender, e) =>
        {
            try
            {
                b(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        };

        if (activateNow)
        {
            try
            {
                b(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

