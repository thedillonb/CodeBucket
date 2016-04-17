using System;
using CodeBucket.DialogElements;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.Utils;
using CodeBucket.Utilities;
using System.Reactive.Linq;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Views.Repositories
{
    public sealed class RepositoriesExploreView : ViewModelCollectionDrivenDialogViewController
    {
		public RepositoriesExploreView()
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

			var vm = (RepositoriesExploreViewModel)ViewModel;
            var search = (UISearchBar)TableView.TableHeaderView;

            vm.GoToRepositoryCommand
                .OfType<RepositoryDetailedModel>()
                .Subscribe(x =>
                {
                    var view = new RepositoryView();
                    view.ViewModel = new RepositoryViewModel(null);
                    NavigationController.PushViewController(view, true);
                });

            vm.Repositories.Changed
                .Select(_ => vm.Repositories.Select(ToElement))
                .Subscribe(x => Root.Reset(new Section { x }));

            OnActivation(d =>
            {
                d(vm.SearchCommand.IsExecuting.SubscribeStatus("Searching..."));
                d(vm.Bind(x => x.SearchText).Subscribe(x => search.Text = x));
                d(search.GetChangedObservable().Subscribe(x => vm.SearchText = x));
                d(search.GetSearchObservable().Subscribe(_ => {
                    search.ResignFirstResponder();
                    vm.SearchCommand.Execute(null);
                }));
            });
        }

        private RepositoryElement ToElement(RepositoryItemViewModel repo)
        {
            var vm = (RepositoriesExploreViewModel)ViewModel;
            var sse = new RepositoryElement(repo.Name, repo.Description, repo.Owner, repo.Avatar);
            sse.Tapped += () => repo.GoToCommand.Execute(repo);
            return sse;
        }
    }
}

