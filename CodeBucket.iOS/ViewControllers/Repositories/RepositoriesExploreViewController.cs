using System;
using CodeBucket.DialogElements;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Utilities;
using System.Reactive.Linq;
using System.Linq;
using BitbucketSharp.Models;
using ReactiveUI;

namespace CodeBucket.Views.Repositories
{
    public sealed class RepositoriesExploreViewController : ViewModelCollectionDrivenDialogViewController<RepositoriesExploreViewModel>
    {
		public RepositoriesExploreViewController()
        {
            Title = "Explore";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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

            ViewModel.Repositories.Changed
                .Select(_ => ViewModel.Repositories.Select(x => new RepositoryElement(x)))
                .Subscribe(x => Root.Reset(new Section { x }));

            OnActivation(d =>
            {
                d(ViewModel.SearchCommand.IsExecuting.SubscribeStatus("Searching..."));
                d(ViewModel.WhenAnyValue(x => x.SearchText).Subscribe(x => search.Text = x));
                d(search.GetChangedObservable().Subscribe(x => ViewModel.SearchText = x));
                d(search.GetSearchObservable().Subscribe(_ => {
                    search.ResignFirstResponder();
                    ViewModel.SearchCommand.Execute(null);
                }));
            });
        }
    }
}

