using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.TableViewSources;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Commits
{
    public abstract class BaseCommitsViewController<TViewModel> : BaseTableViewController<TViewModel, CommitItemViewModel>
        where TViewModel : BaseCommitsViewModel
	{
        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var tableSource = new CommitTableViewSource(TableView, ViewModel.Items);
            tableSource.RequestMore.BindCommand(ViewModel.LoadMoreCommand);
            TableView.Source = tableSource;
		}
	}
}

