using CodeBucket.Core.ViewModels.Repositories;
using System;
using UIKit;
using CodeBucket.Views;
using CodeBucket.TableViewSources;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Repositories
{
    public abstract class BaseRepositoriesViewController<TViewModel> : BaseTableViewController<TViewModel>
        where TViewModel : RepositoriesViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new RepositoryTableViewSource(TableView, ViewModel.Items);
            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."));

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.IsEmpty)
                    .Subscribe(x => TableView.IsEmpty = x)
                    .AddTo(disposable);
            });
        }
    }
}