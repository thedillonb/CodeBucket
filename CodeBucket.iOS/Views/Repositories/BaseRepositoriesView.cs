using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Repositories;
using System;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.TableViewCells;
using System.Reactive.Linq;
using System.Linq;

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

            ViewModel.Repositories.Changed
                .Select(_ => ViewModel.Repositories.Select(ToElement))
                .Subscribe(x => Root.Reset(new Section { x }));
        }

        public override Source CreateSizingSource()
        {
            return new Source(this);
        }

        private Element ToElement(RepositoryItemViewModel repo)
        {
            var sse = new RepositoryElement(repo.Name, repo.Description, repo.Owner, repo.Avatar);
            sse.Tapped += () => repo.GoToCommand.Execute(null);
            return sse;
        }
    }
}