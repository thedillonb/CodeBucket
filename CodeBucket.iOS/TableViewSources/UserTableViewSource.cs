using CodeBucket.Core.ViewModels.Users;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class UserTableViewSource : BaseTableViewSource<UserItemViewModel>
    {
        public UserTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<UserItemViewModel> collection) 
            : base(tableView, collection, UserTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(UserTableViewCell), UserTableViewCell.Key);
        }
    }
}

