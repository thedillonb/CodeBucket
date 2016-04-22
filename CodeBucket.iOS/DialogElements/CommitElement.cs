using Foundation;
using UIKit;
using CodeBucket.TableViewCells;
using CodeBucket.Core.ViewModels.Commits;

namespace CodeBucket.DialogElements
{
    public class CommitElement : Element
    {
        private readonly CommitItemViewModel _viewModel;

        public CommitElement(CommitItemViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView ?? CommitCellView.Create();
            c.Bind(_viewModel.Name, _viewModel.Description, _viewModel.Date, _viewModel.Avatar);
            return c;
        }

        public override bool Matches(string text)
        {
            return _viewModel.Description?.ToLower().Contains(text.ToLower()) ?? false;
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            _viewModel.GoToCommand.Execute(null);
        }
    }
}



