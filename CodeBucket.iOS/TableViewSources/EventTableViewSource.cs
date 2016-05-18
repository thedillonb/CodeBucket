using CodeBucket.Core.ViewModels.Events;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class EventTableViewSource : BaseTableViewSource<EventItemViewModel>
    {
        public EventTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<EventItemViewModel> collection)
            : base(tableView, collection, NewsCellView.Key, UITableView.AutomaticDimension, 80)
        {
            tableView.RegisterNibForCellReuse(NewsCellView.Nib, NewsCellView.Key);
        }
    }
}

