using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class RepositoryTableViewSource : BaseTableViewSource<RepositoryItemViewModel>
    {
        public RepositoryTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<RepositoryItemViewModel> collection)
            : base(tableView, collection, RepositoryCellView.Key, UITableView.AutomaticDimension, 80)
        {
            tableView.RegisterNibForCellReuse(RepositoryCellView.Nib, RepositoryCellView.Key);
        }
    }
}

