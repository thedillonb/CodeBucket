using System;
using CodeBucket.DialogElements;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.Utils;
using CodeBucket.Utilities;

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

			BindCollection(vm.Repositories, repo =>
            {
				var description = vm.ShowRepositoryDescription ? repo.Description : string.Empty;
                var sse = new RepositoryElement(repo.Name, description, repo.Owner, new Avatar(repo.Logo));
				sse.Tapped += () => vm.GoToRepositoryCommand.Execute(repo);
                return sse;
            });

            OnActivation(d =>
            {
                d(vm.Bind(x => x.IsSearching).SubscribeStatus("Searching..."));
                d(vm.Bind(x => x.SearchText).Subscribe(x => search.Text = x));
                d(search.GetChangedObservable().Subscribe(x => vm.SearchText = x));
                d(search.GetSearchObservable().Subscribe(_ => {
                    search.ResignFirstResponder();
                    vm.SearchCommand.Execute(null);
                }));
            });
        }
    }
}

