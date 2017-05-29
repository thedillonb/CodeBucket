using System;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using System.Reactive.Linq;
using ReactiveUI;
using CodeBucket.TableViewSources;

namespace CodeBucket.Views.Repositories
{
    public sealed class RepositoriesExploreViewController : BaseTableViewController<RepositoriesExploreViewModel, RepositoryItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new RepositoryTableViewSource(TableView, ViewModel.Items);
            var search = (UISearchBar)TableView.TableHeaderView;

            OnActivation(d =>
            {
                search
                    .GetSearchObservable()
                    .Do(_ => search.ResignFirstResponder())
                    .BindCommand(ViewModel.SearchCommand)
                    .AddTo(d);
            });
        }
    }
}

