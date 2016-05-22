using System;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Utilities;
using System.Reactive.Linq;
using System.Linq;
using BitbucketSharp.Models;
using ReactiveUI;
using CodeBucket.ViewControllers.Repositories;

namespace CodeBucket.Views.Repositories
{
    public sealed class RepositoriesExploreViewController : BaseTableViewController<RepositoriesExploreViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."));

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            var search = (UISearchBar)TableView.TableHeaderView;

            ViewModel.GoToRepositoryCommand
                .OfType<RepositoryDetailedModel>()
                .Subscribe(x =>
                {
                    var view = new RepositoryViewController();
                    view.ViewModel = new RepositoryViewModel(null, null);
                    NavigationController.PushViewController(view, true);
                });

            OnActivation(d =>
            {
                ViewModel.SearchCommand.IsExecuting.SubscribeStatus("Searching...").AddTo(d);
                ViewModel.WhenAnyValue(x => x.SearchText).Subscribe(x => search.Text = x).AddTo(d);
                search.GetChangedObservable().Subscribe(x => ViewModel.SearchText = x).AddTo(d);
                search.GetSearchObservable().Subscribe(_ => {
                    search.ResignFirstResponder();
                    ViewModel.SearchCommand.Execute(null);
                }).AddTo(d);
            });
        }
    }
}

