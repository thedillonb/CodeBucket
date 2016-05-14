using CodeBucket.Core.ViewModels.Users;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Users
{
    public class UserViewController : PrettyDialogViewController<UserViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var followers = new StringElement("Followers", AtlassianIcon.Star.ToImage());
            followers.BindClick(ViewModel.GoToFollowersCommand);

            var events = new StringElement("Events", AtlassianIcon.Blogroll.ToImage());
            events.BindClick(ViewModel.GoToEventsCommand);

            var groups = new StringElement("Groups", AtlassianIcon.Group.ToImage()) { Hidden = true };
            groups.BindClick(ViewModel.GoToGroupsCommand);

            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            repos.BindClick(ViewModel.GoToRepositoriesCommand);

            var following = new StringElement("Following", AtlassianIcon.View.ToImage());
            following.BindClick(ViewModel.GoToFollowingCommand);

            var website = new StringElement("Website", AtlassianIcon.Weblink.ToImage()) { Hidden = true };
            website.BindClick(ViewModel.GoToWebsiteCommand);

            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, groups, followers, following };
            Root.Reset(midSec, new Section { repos }, new Section { website });

            this.WhenAnyValue(x => x.ViewModel.User)
                .Select(x => x == null ? null : new Avatar(x.Links.Avatar.Href).ToUrl(128))
                .Subscribe(x => HeaderView.SetImage(x, Images.Avatar));

            this.WhenAnyValue(x => x.ViewModel.DisplayName)
                .Subscribe(x => RefreshHeaderView(subtext: x));

            this.WhenAnyValue(x => x.ViewModel.ShouldShowGroups)
                .Subscribe(x => groups.Hidden = !x);

            this.WhenAnyValue(x => x.ViewModel.IsWebsiteAvailable)
                .Subscribe(x => website.Hidden = !x);
        }
    }
}

