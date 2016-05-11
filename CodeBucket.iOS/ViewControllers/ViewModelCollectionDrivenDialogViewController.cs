using System;
using System.Linq;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using CoreGraphics;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;
using CodeBucket.Services;

namespace CodeBucket.ViewControllers
{
    public abstract class ViewModelCollectionDrivenDialogViewController<TViewModel> : ViewModelDrivenDialogViewController<TViewModel>
        where TViewModel : class
    {
        private static NSObject _dumb = new NSObject();

        public Lazy<UIView> EmptyView { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        protected ViewModelCollectionDrivenDialogViewController()
            : base(UITableViewStyle.Plain)
        {
            EnableSearch = true;
        }

        private void CreateEmptyHandler(bool x)
        {
            if (EmptyView == null)
            {
                return;
            }
            if (x)
            {
                if (!EmptyView.IsValueCreated)
                {
                    EmptyView.Value.Alpha = 0f;
                    TableView.AddSubview(EmptyView.Value);
                }

                EmptyView.Value.UserInteractionEnabled = true;
                EmptyView.Value.Frame = new CGRect(0, 0, TableView.Bounds.Width, TableView.Bounds.Height);
                TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                TableView.BringSubviewToFront(EmptyView.Value);
                if (TableView.TableHeaderView != null)
                    TableView.TableHeaderView.Hidden = true;
                UIView.Animate(0.2f, 0f, UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState,
                    () => EmptyView.Value.Alpha = 1.0f, null);
            }
            else if (EmptyView.IsValueCreated)
            {
                EmptyView.Value.UserInteractionEnabled = false;
                if (TableView.TableHeaderView != null)
                    TableView.TableHeaderView.Hidden = false;
                TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
                UIView.Animate(0.1f, 0f, UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState,
                    () => EmptyView.Value.Alpha = 0f, null);
            }
        }

        protected ICollection<Section> RenderList<T>(IEnumerable<T> items, Func<T, Element> select, Func<Task> moreAction)
        {
            items = items ?? Enumerable.Empty<T>();
            var sec = new Section();
            sec.AddAll(items.Select(item =>
            {
                try
                {
                    return @select(item);
                }
                catch
                {
                    return null;
                }
            }).Where(x => x != null));

            return RenderSections(new [] { sec }, moreAction);
        }

        protected virtual Section CreateSection(string text)
        {
            return new Section(text);
        }

        protected ICollection<Section> RenderGroupedItems<T>(IEnumerable<IGrouping<string, T>> items, Func<T, Element> select, Func<Task> moreAction)
        {
            var sections = new List<Section>();

            if (items != null)
            {
                foreach (var grp in items.ToList())
                {
                    try
                    {
                        var sec = CreateSection(grp.Key);
                        foreach (var element in grp.Select(select).Where(element => element != null))
                            sec.Add(element);

                        if (sec.Elements.Count > 0)
                            sections.Add(sec);
                    }
                    catch 
                    {
                    }
                }
            }

            return RenderSections(sections, moreAction);
        }

        private static ICollection<Section> RenderSections(IEnumerable<Section> sections, Func<Task> moreAction)
        {
            var weakAction = new WeakReference<Func<Task>>(moreAction);
            ICollection<Section> newSections = new LinkedList<Section>(sections);

            if (moreAction != null)
            {
                var loadMore = new PaginateElement("Load More", "Loading...") { AutoLoadOnVisible = true };
                newSections.Add(new Section { loadMore });
                loadMore.Tapped += async (obj) =>
                {
                    try
                    {
                        NetworkActivity.PushNetworkActive();

                        var a = weakAction.Get();
                        if (a != null)
                            await a();

                        var root = loadMore.GetRootElement();
                        root?.Remove(loadMore.Section, UITableViewRowAnimation.Fade);
                    }
                    catch (Exception e)
                    {
                        AlertDialogService.ShowAlert("Unable to load more!", e.Message);
                    }
                    finally
                    {
                        NetworkActivity.PopNetworkActive();
                    }

                };    
            }

            return newSections;
        }
    }
}

