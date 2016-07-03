using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class CommitTableViewSource : BaseTableViewSource<CommitItemViewModel>
    {
        public CommitTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<CommitItemViewModel> collection) 
            : base(tableView, collection, CommitCellView.Key, UITableView.AutomaticDimension, 80)
        {
            tableView.RegisterNibForCellReuse(CommitCellView.Nib, CommitCellView.Key);
        }
    }
}

