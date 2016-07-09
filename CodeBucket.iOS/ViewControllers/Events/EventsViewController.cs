using System;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.TableViewSources;
using CodeBucket.Views;
using ReactiveUI;
using UIKit;

namespace CodeBucket.ViewControllers.Events
{
    public abstract class EventsViewController<TViewModel> : BaseTableViewController<TViewModel, EventItemViewModel>
        where TViewModel : BaseEventsViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hide the search bar
            TableView.TableHeaderView = null;

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Blogroll.ToEmptyListImage(), "There are no events."));
            var source = new EventTableViewSource(TableView, ViewModel.Items);
            source.RequestMore.InvokeCommand(ViewModel.LoadMoreCommand);
            TableView.Source = source;
        }
    }
}