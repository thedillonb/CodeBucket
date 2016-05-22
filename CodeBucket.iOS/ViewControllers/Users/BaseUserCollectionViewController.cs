using CodeBucket.Core.ViewModels.Users;
using UIKit;
using System;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Users
{
    public abstract class BaseUserCollectionViewController<TViewModel> : BaseTableViewController<TViewModel, UserItemViewModel> 
        where TViewModel : UsersViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var emptyMessage = ViewModel.EmptyMessage ?? "There are no users.";
            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.User.ToEmptyListImage(), emptyMessage));
            TableView.Source = new UserTableViewSource(TableView, ViewModel.Items);
        }
    }
}

