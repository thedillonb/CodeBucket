using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Repositories;
using System;
using UIKit;
using CodeBucket.TableViewCells;
using System.Reactive.Linq;
using System.Linq;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Repositories
{
    public abstract class BaseRepositoriesViewController<TViewModel> : BaseViewController<TViewModel>
        where TViewModel : RepositoriesViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableView = new EnhancedTableView(UITableViewStyle.Plain)
            {
                ViewModel = ViewModel,
                EmptyView = new Lazy<UIView>(() =>
                    new EmptyListView(AtlassianIcon.Devtoolsrepository.ToEmptyListImage(), "There are no repositories."))
            };

            tableView.RegisterNibForCellReuse(RepositoryCellView.Nib, RepositoryCellView.Key);
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 80f;

            this.AddTableView(tableView);
            var root = new RootElement(tableView);
            tableView.Source = new DialogElementTableViewSource(root);

            ViewModel.Items.ChangedObservable()
                .Select(x => x.Select(y => new RepositoryElement(y)))
                .Subscribe(x => root.Reset(new Section { x }));
        }
    }
}