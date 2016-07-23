using CodeBucket.Core.ViewModels.Repositories;
using System;
using UIKit;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Repositories
{
    public abstract class RepositoriesViewController<TViewModel> : BaseTableViewController<TViewModel, RepositoryItemViewModel>
        where TViewModel : RepositoriesViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new RepositoryTableViewSource(TableView, ViewModel.Items);
            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."));
        }
    }
}