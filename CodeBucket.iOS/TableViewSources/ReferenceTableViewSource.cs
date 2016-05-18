using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class ReferenceTableViewSource : BaseTableViewSource<ReferenceItemViewModel>
    {
        public ReferenceTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<ReferenceItemViewModel> collection)
            : base(tableView, collection, ReferenceTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(ReferenceTableViewCell), ReferenceTableViewCell.Key);
        }
    }
}

