using CodeFramework.iOS.Elements;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using MonoTouch.Dialog;
using CodeBucket.iOS.Views.Filters;
using BitbucketSharp.Models;
using System;

namespace CodeBucket.iOS.Views.Repositories
{
    public abstract class BaseRepositoriesView : ViewModelCollectionDrivenViewController
    {
        public new RepositoriesViewModel ViewModel
        {  
            get { return (RepositoriesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        protected BaseRepositoriesView()
        {
            Title = "Repositories".t();
            NoItemsText = "No Repositories".t(); 
			NavigationItem.RightBarButtonItem = new MonoTouch.UIKit.UIBarButtonItem(Theme.CurrentTheme.SortButton, MonoTouch.UIKit.UIBarButtonItemStyle.Plain, 
				(s, e) => ShowFilterController(new RepositoriesFilterViewController(ViewModel.Repositories)));  
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BindCollection(ViewModel.Repositories, CreateElement);
			TableView.SeparatorInset = new MonoTouch.UIKit.UIEdgeInsets(0, 56f, 0, 0);
        }

		protected Element CreateElement(RepositoryDetailedModel repo)
        {
            var description = ViewModel.ShowRepositoryDescription ? repo.Description : string.Empty;
			var sse = new RepositoryElement(repo.Name, (uint)repo.FollowersCount, (uint)repo.ForkCount, description, repo.Owner, new Uri(repo.LargeLogo(64))) { ShowOwner = ViewModel.ShowRepositoryOwner };
            sse.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(repo);
            return sse;
        }
    }
}