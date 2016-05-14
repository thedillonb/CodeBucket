using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Users;
using UIKit;
using System;
using CodeBucket.Views;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.User
{
    public abstract class BaseUserCollectionViewController<TViewModel> : BaseViewController<TViewModel> 
        where TViewModel : BaseUserCollectionViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var emptyMessage = ViewModel.EmptyMessage ?? "There are no users.";
            var tableView = new EnhancedTableView(UITableViewStyle.Plain)
            {
                ViewModel = ViewModel,
                EmptyView = new Lazy<UIView>(() =>
                    new EmptyListView(AtlassianIcon.User.ToEmptyListImage(), emptyMessage))
            };

            this.AddTableView(tableView);
            var root = new RootElement(tableView);
            tableView.Source = new DialogElementTableViewSource(root);

            ViewModel.Items.ChangedObservable()
              .Select(users => users.Select(y => new UserElement(y)))
              .Subscribe(elements => root.Reset(new Section { elements }));
        }
    }
}

