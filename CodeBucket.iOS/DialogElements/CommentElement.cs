using UIKit;
using System;
using CodeBucket.TableViewCells;
using CodeBucket.Core.Utils;
using Humanizer;

namespace CodeBucket.DialogElements
{
    public class CommentElement : Element
    {
        private readonly Core.ViewModels.Commits.CommitItemViewModel _viewModel;

        public CommentElement(string title, string message, DateTimeOffset date, Avatar avatar)
        {
            _viewModel = new Core.ViewModels.Commits.CommitItemViewModel(title, message, date.Humanize(), avatar);
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

