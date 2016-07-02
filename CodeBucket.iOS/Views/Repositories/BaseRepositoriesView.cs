using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Client.Models;
using System;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.TableViewCells;
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
            Title = "Repositories";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."));
        }

        public override void ViewDidLoad()
        {
            TableView.RegisterNibForCellReuse(RepositoryCellView.Nib, RepositoryCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            base.ViewDidLoad();

            BindCollection(ViewModel.Repositories, CreateElement);
        }

        public override Source CreateSizingSource()
        {
            return new Source(this);
        }

        protected Element CreateElement(Repository repo)
        {
            var description = ViewModel.ShowRepositoryDescription ? repo.Description : string.Empty;
            var sse = new RepositoryElement(repo.Name, description, repo.Owner.Username, new Avatar(repo.Owner?.Links?.Avatar?.Href));
            sse.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(repo);
            return sse;
        }
    }
}