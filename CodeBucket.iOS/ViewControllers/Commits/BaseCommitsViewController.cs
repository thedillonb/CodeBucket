using CodeBucket.TableViewCells;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Commits;

namespace CodeBucket.ViewControllers.Commits
{
	public abstract class BaseCommitsViewController : ViewModelCollectionDrivenDialogViewController
	{
		public override void ViewDidLoad()
		{
			Title = "Commits";

			base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(CommitCellView.Nib, CommitCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            var vm = (ICommitsViewModel)ViewModel;
            BindCollection(vm.Commits, x => new CommitElement(x));
		}
	}
}

