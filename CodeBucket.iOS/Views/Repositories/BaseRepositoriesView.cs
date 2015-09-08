using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views.Filters;
using BitbucketSharp.Models;
using System;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.Cells;

namespace CodeBucket.Views.Repositories
{
    public abstract class BaseRepositoriesView : ViewModelCollectionDrivenDialogViewController
    {
        public new RepositoriesViewModel ViewModel
        {  
            get { return (RepositoriesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            NoItemsText = "No Repositories"; 
            Title = "Repositories";

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
            var sse = new RepositoryElement(repo.Name, description, repo.Owner, new Uri(repo.LargeLogo(64)), Images.RepoPlaceholder);
            sse.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(repo);
            return sse;
        }
    }
}