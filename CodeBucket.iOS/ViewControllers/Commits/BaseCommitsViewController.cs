using System;
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
            TableView.Source = tableSource;

            OnActivation(disposable =>
            {
                tableSource.RequestMore
                    .InvokeCommand(ViewModel.LoadMoreCommand)
                    .AddTo(disposable);
            });
		}
	}
}

