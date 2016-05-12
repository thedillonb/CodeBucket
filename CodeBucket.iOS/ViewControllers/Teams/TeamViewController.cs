using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamViewController : PrettyDialogViewController<TeamViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var followers = new StringElement("Followers", AtlassianIcon.Star.ToImage());
            var events = new StringElement("Events", AtlassianIcon.Blogroll.ToImage());
            var organizations = new StringElement("Groups", AtlassianIcon.Group.ToImage());
            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            var members = new StringElement("Members", AtlassianIcon.User.ToImage());
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, members, followers };
            Root.Reset(midSec, new Section { repos });

            followers.Clicked.InvokeCommand(ViewModel.GoToFollowersCommand);
            events.Clicked.InvokeCommand(ViewModel.GoToEventsCommand);
            organizations.Clicked.InvokeCommand(ViewModel.GoToGroupsCommand);
            repos.Clicked.InvokeCommand(ViewModel.GoToRepositoriesCommand);
            members.Clicked.InvokeCommand(ViewModel.GoToMembersCommand);

            this.WhenAnyValue(x => x.ViewModel.Team).Subscribe(x =>
            {
                if (x == null)
                {
                    HeaderView.SetImage(null, Images.Avatar);
                }
                else
                {
                    HeaderView.SubText = string.IsNullOrWhiteSpace(x.DisplayName) ? x.Username : x.DisplayName;
                    HeaderView.SetImage(new Avatar(x.Links.Avatar.Href).ToUrl(128), Images.Avatar);
                    RefreshHeaderView();
                }
            });
        }
    }
}

