using CodeBucket.Core.ViewModels.Events;
using CodeBucket.TableViewSources;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Events
{
    public abstract class BaseEventsViewController<TViewModel> : BaseTableViewController<TViewModel>
        where TViewModel : BaseEventsViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new EventTableViewSource(TableView, ViewModel.Items);
            TableView.Source = source;
            source.RequestMore.InvokeCommand(ViewModel.LoadMoreCommand);
        }
    }
}