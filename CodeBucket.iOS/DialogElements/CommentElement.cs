using UIKit;
using CodeBucket.TableViewCells;
using CodeBucket.Core.ViewModels.Comments;

namespace CodeBucket.DialogElements
{
    public class CommentElement : Element
    {
        private readonly Core.ViewModels.Commits.CommitItemViewModel _viewModel;

        public CommentElement(CommentItemViewModel viewModel)
        {
            _viewModel = new Core.ViewModels.Commits.CommitItemViewModel(
                viewModel.Name, viewModel.Content, viewModel.CreatedOn, viewModel.Avatar, null);
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView ?? CommitCellView.Create();
            c.ViewModel = _viewModel;
            return c;
        }

        public override bool Matches(string text)
        {
            return _viewModel.Description?.ToLower().Contains(text.ToLower()) ?? false;
        }
    }
}

