using System;
using System.Drawing;
using CodeFramework.iOS.Elements;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace CodeBucket.iOS.Views.Repositories
{
    public sealed class RepositoriesExploreView : ViewModelCollectionDrivenViewController
    {
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

			BindCollection(vm.Repositories, repo =>
            {
				var description = vm.ShowRepositoryDescription ? repo.Description : string.Empty;
				var imageUrl = new Uri(repo.Logo);
				var sse = new RepositoryElement(repo.Name, (uint)repo.ForkCount, (uint)repo.ForkCount, description, repo.Owner, imageUrl) { ShowOwner = true };
				sse.Tapped += () => vm.GoToRepositoryCommand.Execute(repo);
                return sse;
            });
        }
    }
}

