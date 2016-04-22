using Foundation;
using UIKit;
using CodeBucket.TableViewCells;
using CodeBucket.Core.ViewModels.PullRequests;

namespace CodeBucket.DialogElements
{
    public class PullRequestElement : Element
    {
        private readonly PullRequestItemViewModel _viewModel;

        public PullRequestElement(PullRequestItemViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(PullRequestCellView.Key) as PullRequestCellView ?? PullRequestCellView.Create();
            c.Bind(_viewModel.Title, _viewModel.CreatedOn, _viewModel.Avatar);
            return c;
        }

        public override bool Matches(string text)
        {
            return _viewModel.Title.ToLower().Contains(text.ToLower());
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            _viewModel.GoToCommand.Execute(null);
        }
    }
}

