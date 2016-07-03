using CodeBucket.Core.ViewModels.PullRequests;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class PullRequestTableViewSource : BaseTableViewSource<PullRequestItemViewModel>
    {
        public PullRequestTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<PullRequestItemViewModel> collection)
            : base(tableView, collection, PullRequestCellView.Key, UITableView.AutomaticDimension, 80)
        {
            tableView.RegisterNibForCellReuse(PullRequestCellView.Nib, PullRequestCellView.Key);
        }
    }
}

