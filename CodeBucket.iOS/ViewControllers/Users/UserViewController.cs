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
            var events = new ButtonElement("Events", AtlassianIcon.Blogroll.ToImage());
            var groups = new ButtonElement("Groups", AtlassianIcon.Group.ToImage());
            var repos = new ButtonElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            var following = new ButtonElement("Following", AtlassianIcon.View.ToImage());
            var websiteSection = new Section();
            var website = new ButtonElement("Website", AtlassianIcon.Weblink.ToImage());
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, followers, following };
            Root.Reset(midSec, new Section { repos }, websiteSection);

            OnActivation(disposable =>
            {
                followers.BindClick(ViewModel.GoToFollowersCommand).AddTo(disposable);
                events.BindClick(ViewModel.GoToEventsCommand).AddTo(disposable);
                groups.BindClick(ViewModel.GoToGroupsCommand).AddTo(disposable);
                repos.BindClick(ViewModel.GoToRepositoriesCommand).AddTo(disposable);
                following.BindClick(ViewModel.GoToFollowingCommand).AddTo(disposable);
                website.BindClick(ViewModel.GoToWebsiteCommand).AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.User)
                    .Select(x => x == null ? null : new Avatar(x.Links.Avatar.Href).ToUrl(128))
                    .Subscribe(x => HeaderView.SetImage(x, Images.Avatar))
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.DisplayName)
                    .Subscribe(x => RefreshHeaderView(subtext: x))
                    .AddTo(disposable);
            });

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

