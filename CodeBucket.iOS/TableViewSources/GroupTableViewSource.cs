using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class GroupTableViewSource : BaseTableViewSource<GroupItemViewModel>
    {
        public GroupTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<GroupItemViewModel> collection)
            : base(tableView, collection, GroupTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(GroupTableViewCell), GroupTableViewCell.Key);
        }
    }
}

