using UIKit;
using CodeBucket.Core.ViewModels.Users;
using ReactiveUI;
using CodeBucket.TableViewCells;

namespace CodeBucket.DialogElements
{
    public class UserElement : Element
    {
        private readonly UserItemViewModel _viewModel;

        public UserElement(UserItemViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = (tv.DequeueReusableCell(UserTableViewCell.Key) as UserTableViewCell) ?? new UserTableViewCell();
            cell.ViewModel = _viewModel;
            return cell;
        }

        public override void Selected(UITableView tableView, Foundation.NSIndexPath path)
        {
            base.Selected(tableView, path);
            _viewModel.GoToCommand.ExecuteNow();
        }
    }
}

