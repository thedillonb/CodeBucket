using CodeBucket.Core.ViewModels.Teams;
using System;
using UIKit;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamsViewController : BaseTableViewController<TeamsViewModel, TeamItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Userstatus.ToEmptyListImage(), "There are no teams."));
            TableView.Source = new TeamTableViewSource(TableView, ViewModel.Items);
        }
    }
}