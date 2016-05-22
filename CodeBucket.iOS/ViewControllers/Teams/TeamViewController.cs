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

            var followers = new ButtonElement("Followers", AtlassianIcon.Star);
            var events = new ButtonElement("Events", AtlassianIcon.Blogroll);
            var organizations = new ButtonElement("Groups", AtlassianIcon.Group);
            var repos = new ButtonElement("Repositories", AtlassianIcon.Devtoolsrepository);
            var members = new ButtonElement("Members", AtlassianIcon.User);
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, members, followers };
            Root.Reset(midSec, new Section { repos });

            OnActivation(disposable =>
            {
                followers.BindClick(ViewModel.GoToFollowersCommand).AddTo(disposable);
                events.BindClick(ViewModel.GoToEventsCommand).AddTo(disposable);
                organizations.BindClick(ViewModel.GoToGroupsCommand).AddTo(disposable);
                repos.BindClick(ViewModel.GoToRepositoriesCommand).AddTo(disposable);
                members.BindClick(ViewModel.GoToMembersCommand).AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.Team)
                    .Select(x => x == null ? null : new Avatar(x.Links.Avatar.Href).ToUrl(128))
                    .Subscribe(x => HeaderView.SetImage(x, Images.Avatar))
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.DisplayName)
                    .Subscribe(x => RefreshHeaderView(subtext: x))
                    .AddTo(disposable);
            });
        }
    }
}

