using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views.Filters;
using BitbucketSharp.Models;
using System;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.Cells;
using CodeBucket.Core.Utils;

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
            NoItemsText = "No Repositories"; 
            Title = "Repositories";
        }

        public override void ViewDidLoad()
        {
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.SortButton, UIBarButtonItemStyle.Plain, 
                (s, e) => ShowFilterController(new RepositoriesFilterViewController(ViewModel.Repositories)));  

            TableView.RegisterNibForCellReuse(RepositoryCellView.Nib, RepositoryCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            base.ViewDidLoad();

            BindCollection(ViewModel.Repositories, CreateElement);
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new DialogViewController.Source(this);
        }

		protected Element CreateElement(RepositoryDetailedModel repo)
        {
            var description = ViewModel.ShowRepositoryDescription ? repo.Description : string.Empty;
            var avatarUrl = new Avatar(repo.Logo).ToUrl();
            var sse = new RepositoryElement(repo.Name, description, repo.Owner, avatarUrl, Images.RepoPlaceholder);
            sse.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(repo);
            return sse;
        }
    }
}