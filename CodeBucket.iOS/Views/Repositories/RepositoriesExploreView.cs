using System;
using System.Drawing;
using CodeFramework.iOS.Elements;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using CodeFramework.iOS.Utils;

namespace CodeBucket.iOS.Views.Repositories
{
    public sealed class RepositoriesExploreView : ViewModelCollectionDrivenDialogViewController
    {
		private Hud _hud;
		public RepositoriesExploreView()
        {
            AutoHideSearch = false;
            //EnableFilter = true;
            NoItemsText = "No Repositories".t();
            Title = "Explore".t();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			_hud = new Hud(View);
			var vm = (RepositoriesExploreViewModel)ViewModel;
            var search = (UISearchBar)TableView.TableHeaderView;

			var set = this.CreateBindingSet<RepositoriesExploreView, RepositoriesExploreViewModel>();
			set.Bind(search).For(x => x.Text).To(x => x.SearchText);
			set.Apply();

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

