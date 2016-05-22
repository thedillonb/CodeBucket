using System;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.TableViewSources;
using CodeBucket.Views;
using UIKit;

namespace CodeBucket.ViewControllers.Groups
{
    public class GroupsViewController : BaseTableViewController<GroupsViewModel, GroupItemViewModel>
	{
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Group.ToEmptyListImage(), "There are no groups."));
            TableView.Source = new GroupTableViewSource(TableView, ViewModel.Items);
        }
	}
}

