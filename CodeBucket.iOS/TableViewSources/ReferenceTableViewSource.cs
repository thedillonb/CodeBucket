using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class ReferenceTableViewSource : BaseTableViewSource<GitReferenceItemViewModel>
    {
        public ReferenceTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<GitReferenceItemViewModel> collection)
            : base(tableView, collection, ReferenceTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(ReferenceTableViewCell), ReferenceTableViewCell.Key);
        }
    }
}

