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

            var followers = new ButtonElement("Followers", AtlassianIcon.Star.ToImage());
            followers.BindClick(ViewModel.GoToFollowersCommand);

            var events = new ButtonElement("Events", AtlassianIcon.Blogroll.ToImage());
            events.BindClick(ViewModel.GoToEventsCommand);

            var groups = new ButtonElement("Groups", AtlassianIcon.Group.ToImage());
            groups.BindClick(ViewModel.GoToGroupsCommand);

            var repos = new ButtonElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            repos.BindClick(ViewModel.GoToRepositoriesCommand);

            var following = new ButtonElement("Following", AtlassianIcon.View.ToImage());
            following.BindClick(ViewModel.GoToFollowingCommand);

            var websiteSection = new Section();
            var website = new ButtonElement("Website", AtlassianIcon.Weblink.ToImage());
            website.BindClick(ViewModel.GoToWebsiteCommand);

            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, followers, following };
            Root.Reset(midSec, new Section { repos }, websiteSection);

            this.WhenAnyValue(x => x.ViewModel.User)
                .Select(x => x == null ? null : new Avatar(x.Links.Avatar.Href).ToUrl(128))
                .Subscribe(x => HeaderView.SetImage(x, Images.Avatar));

            this.WhenAnyValue(x => x.ViewModel.DisplayName)
                .Subscribe(x => RefreshHeaderView(subtext: x));

            this.WhenAnyValue(x => x.ViewModel.ShouldShowGroups)
                .Subscribe(x =>
                {
                    if (x)
                        midSec.Insert(1, UITableViewRowAnimation.Automatic, groups);
                    else
                        midSec.Remove(groups);
                });

            this.WhenAnyValue(x => x.ViewModel.IsWebsiteAvailable)
                .Subscribe(x =>
                {
                    if (x)
                        websiteSection.Insert(0, UITableViewRowAnimation.Automatic, website);
                    else
                        websiteSection.Remove(website);
                });
        }
    }
}

