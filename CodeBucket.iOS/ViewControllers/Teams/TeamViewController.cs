using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamViewController : PrettyDialogViewController<TeamViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var followers = new StringElement("Followers", AtlassianIcon.Star);
            var events = new StringElement("Events", AtlassianIcon.Blogroll);
            var organizations = new StringElement("Groups", AtlassianIcon.Group);
            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository);
            var members = new StringElement("Members", AtlassianIcon.User);
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, members, followers };
            Root.Reset(midSec, new Section { repos });

            followers.BindClick(ViewModel.GoToFollowersCommand);
            events.BindClick(ViewModel.GoToEventsCommand);
            organizations.BindClick(ViewModel.GoToGroupsCommand);
            repos.BindClick(ViewModel.GoToRepositoriesCommand);
            members.BindClick(ViewModel.GoToMembersCommand);

            this.WhenAnyValue(x => x.ViewModel.Team)
                .Select(x => x == null ? null : new Avatar(x.Links.Avatar.Href).ToUrl(128))
                .Subscribe(x => HeaderView.SetImage(x, Images.Avatar));

            this.WhenAnyValue(x => x.ViewModel.DisplayName)
                .Subscribe(x => RefreshHeaderView(subtext: x));
        }
    }
}

