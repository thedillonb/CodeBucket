using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;

namespace CodeBucket.Views.Teams
{
    public class TeamView : PrettyDialogViewController
    {
        public new TeamViewModel ViewModel
        {
            get { return (TeamViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Name;

            var followers = new StringElement("Followers", AtlassianIcon.Star.ToImage());
            var events = new StringElement("Events", AtlassianIcon.Blogroll.ToImage());
            var organizations = new StringElement("Groups", AtlassianIcon.Group.ToImage());
            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            var members = new StringElement("Members", AtlassianIcon.User.ToImage());
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, members, followers };
            Root.Reset(midSec, new Section { repos });

            OnActivation(d => 
            {
                d(followers.Clicked.BindCommand(ViewModel.GoToFollowersCommand));
                d(events.Clicked.BindCommand(ViewModel.GoToEventsCommand));
                d(organizations.Clicked.BindCommand(ViewModel.GoToGroupsCommand));
                d(repos.Clicked.BindCommand(ViewModel.GoToRepositoriesCommand));
                d(members.Clicked.BindCommand(ViewModel.GoToMembersCommand));

                d(ViewModel.Bind(x => x.User, true).Subscribe(x =>
                {
                    if (x == null)
                    {
                        HeaderView.SetImage(null, Images.Avatar);
                    }
                    else
                    {
                        HeaderView.SubText = string.IsNullOrWhiteSpace(x.DisplayName) ? x.Username : x.DisplayName;
                        HeaderView.SetImage(new Avatar(x?.Links?.Avatar?.Href).ToUrl(128), Images.Avatar);
                        RefreshHeaderView();
                    }
                }));
            });
        }
    }
}

