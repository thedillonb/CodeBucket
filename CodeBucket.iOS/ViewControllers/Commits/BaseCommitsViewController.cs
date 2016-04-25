using System;
using CodeBucket.TableViewCells;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Commits;
using System.Reactive.Linq;
using System.Linq;

namespace CodeBucket.ViewControllers.Commits
{
	public abstract class BaseCommitsViewController : ViewModelDrivenDialogViewController
	{
        protected BaseCommitsViewController()
            : base(style: UITableViewStyle.Plain)
        {
        }

		public override void ViewDidLoad()
		{
			Title = "Commits";

			base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(CommitCellView.Nib, CommitCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            var vm = (ICommitsViewModel)ViewModel;

            vm.Commits
              .ChangedObservable()
              .Select(x => x.Select(y => new CommitElement(y)))
              .Subscribe(x => Root.Reset(new Section { x }));

            EndOfList.BindCommand(vm.LoadMoreCommand);

            vm.LoadMoreCommand.IsExecuting.Subscribe(x =>
            {
                if (x)
                {
                    var activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                    activity.Frame = new CoreGraphics.CGRect(0, 0, 320, 64f);
                    activity.StartAnimating();
                    TableView.TableFooterView = activity;
                }
                else
                {
                    TableView.TableFooterView = null;
                }
            });
		}
	}
}

