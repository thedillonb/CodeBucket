using System;
using Foundation;
using UIKit;
using CodeBucket.TableViewCells;
using CodeBucket.Core.ViewModels.Repositories;

namespace CodeBucket.DialogElements
{
    public class RepositoryElement : Element
    {
        private readonly RepositoryItemViewModel _viewModel;

        public bool ShowOwner { get; set; }

        public RepositoryElement(RepositoryItemViewModel viewModel)
        {
            _viewModel = viewModel;
            ShowOwner = true;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(RepositoryCellView.Key) as RepositoryCellView ?? RepositoryCellView.Create();
            cell.Bind(_viewModel.Name, _viewModel.Description, ShowOwner ? _viewModel.Owner : null, _viewModel.Avatar);
            return cell;
        }
        
        public override bool Matches(string text)
        {
            var name = _viewModel.Name ?? string.Empty;
            return name.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1;
        }
        
        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            _viewModel.GoToCommand.Execute(null);
        }
    }
}

