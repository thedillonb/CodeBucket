using System;
using CodeBucket.TableViewCells;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Commits;
using System.Reactive.Linq;
using System.Linq;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Commits
{
    public abstract class BaseCommitsViewController<TViewModel> : BaseViewController<TViewModel>
        where TViewModel : BaseCommitsViewModel
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var tableView = new EnhancedTableView { ViewModel = ViewModel };
            tableView.RegisterNibForCellReuse(CommitCellView.Nib, CommitCellView.Key);
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 80f;
            this.AddTableView(tableView);

            var root = new RootElement(tableView);
            var tableViewSource = new DialogElementTableViewSource(root);
            tableView.Source = tableViewSource;

            ViewModel.Items
                .ChangedObservable()
                .Select(x => x.Select(y => new CommitElement(y)))
                .Subscribe(x => root.Reset(new Section { x }));

            OnActivation(d =>
            {
                d(tableViewSource.EndOfList.BindCommand(ViewModel.LoadMoreCommand));
            });
		}
	}
}

