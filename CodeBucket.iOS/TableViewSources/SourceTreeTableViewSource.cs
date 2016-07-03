using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class SourceTreeTableViewSource : BaseTableViewSource<SourceTreeItemViewModel>
    {
        public SourceTreeTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<SourceTreeItemViewModel> collection)
            : base(tableView, collection, SourceTreeTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(SourceTreeTableViewCell), SourceTreeTableViewCell.Key);
        }
    }
}

