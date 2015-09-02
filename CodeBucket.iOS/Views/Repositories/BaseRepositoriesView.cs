using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views.Filters;
using BitbucketSharp.Models;
using System;
using CodeBucket.ViewControllers;

namespace CodeBucket.Views.Repositories
{
    public abstract class BaseRepositoriesView : ViewModelCollectionDrivenDialogViewController
    {
        public new RepositoriesViewModel ViewModel
        {  
            get { return (RepositoriesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        protected BaseRepositoriesView()
        {
            Title = "Repositories";
            NoItemsText = "No Repositories"; 
			NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem(Theme.CurrentTheme.SortButton, UIKit.UIBarButtonItemStyle.Plain, 
				(s, e) => ShowFilterController(new RepositoriesFilterViewController(ViewModel.Repositories)));  
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BindCollection(ViewModel.Repositories, CreateElement);
			TableView.SeparatorInset = new UIKit.UIEdgeInsets(0, 56f, 0, 0);
        }

		protected Element CreateElement(RepositoryDetailedModel repo)
        {
            var description = ViewModel.ShowRepositoryDescription ? repo.Description : string.Empty;
			var sse = new RepositoryElement(repo.Name, repo.FollowersCount, repo.ForkCount, description, repo.Owner, new Uri(repo.LargeLogo(64))) { ShowOwner = ViewModel.ShowRepositoryOwner };
            sse.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(repo);
            return sse;
        }
    }
}