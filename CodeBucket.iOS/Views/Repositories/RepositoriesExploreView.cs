using System;
using CodeBucket.Elements;
using UIKit;
using CodeBucket.Utils;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;

namespace CodeBucket.Views.Repositories
{
    public sealed class RepositoriesExploreView : ViewModelCollectionDrivenDialogViewController
    {
		private Hud _hud;
		public RepositoriesExploreView()
        {
            AutoHideSearch = false;
            //EnableFilter = true;
            NoItemsText = "No Repositories";
            Title = "Explore";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			_hud = new Hud(View);
			var vm = (RepositoriesExploreViewModel)ViewModel;
            var search = (UISearchBar)TableView.TableHeaderView;

            search.TextChanged += (sender, e) => vm.SearchText = search.Text;
            vm.Bind(x => x.SearchText, x => search.Text = x);
			search.SearchButtonClicked += (sender, e) =>
			{
				search.ResignFirstResponder();
				vm.SearchCommand.Execute(null);
			};

			vm.Bind(x => x.IsSearching, x =>
			{
				if (x)
					_hud.Show("Searching...");
				else
					_hud.Hide();
			});

			BindCollection(vm.Repositories, repo =>
            {
				var description = vm.ShowRepositoryDescription ? repo.Description : string.Empty;
				var imageUrl = new Uri(repo.Logo);
					var sse = new RepositoryElement(repo.Name, repo.FollowersCount, repo.ForkCount, description, repo.Owner, imageUrl) { ShowOwner = true };
				sse.Tapped += () => vm.GoToRepositoryCommand.Execute(repo);
                return sse;
            });
        }
    }
}

