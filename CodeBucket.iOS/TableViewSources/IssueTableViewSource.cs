using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class IssueTableViewSource : BaseTableViewSource<IssueItemViewModel>
    {
        public IssueTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<IssueItemViewModel> collection)
            : base(tableView, collection, IssueCellView.Key, 69f)
        {
            tableView.RegisterNibForCellReuse(IssueCellView.Nib, IssueCellView.Key);
        }
    }
}

