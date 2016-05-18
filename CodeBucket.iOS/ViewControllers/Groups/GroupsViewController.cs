using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Groups
{
    public class GroupsViewController : BaseTableViewController<GroupsViewModel>
	{
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new GroupTableViewSource(TableView, ViewModel.Items);
        }
	}
}

