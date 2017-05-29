using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class AccountTableViewSource : BaseTableViewSource<AccountItemViewModel>
    {
        public AccountTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<AccountItemViewModel> collection)
            : base(tableView, collection, AccountTableViewCell.Key, 74f)
        {
            tableView.SeparatorInset = new UIEdgeInsets(0, tableView.RowHeight, 0, 0);
            tableView.RegisterClassForCellReuse(typeof(AccountTableViewCell), AccountTableViewCell.Key);
        }

        public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
        {
            var vm = ItemAt(indexPath) as AccountItemViewModel;
            vm?.DeleteCommand.ExecuteNow();
        }
    }
}

