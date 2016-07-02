using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Views.User
{
    public class ProfileView : PrettyDialogViewController
    {
		public new ProfileViewModel ViewModel
		{
			get { return (ProfileViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.Text = ViewModel.Username;
            Title = ViewModel.Username;

            var followers = new StringElement("Followers", AtlassianIcon.Star.ToImage());
            var events = new StringElement("Events", AtlassianIcon.Blogroll.ToImage());
            var organizations = new StringElement("Groups", AtlassianIcon.Group.ToImage());
            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            var following = new StringElement("Following", AtlassianIcon.View.ToImage());
            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, followers, following };
            Root.Reset(midSec, new Section { repos });

            OnActivation(d =>
            {
                d(followers.Clicked.BindCommand(ViewModel.GoToFollowersCommand));
                d(events.Clicked.BindCommand(ViewModel.GoToEventsCommand));
                d(organizations.Clicked.BindCommand(ViewModel.GoToGroupsCommand));
                d(repos.Clicked.BindCommand(ViewModel.GoToRepositoriesCommand));
                d(following.Clicked.BindCommand(ViewModel.GoToFollowingCommand));

                d(ViewModel.Bind(x => x.User, true).IsNotNull().Subscribe(x =>
                {
                    if (x == null)
                    {
                        HeaderView.SetImage(null, Images.Avatar);
                    }
                    else
                    {
                        var name = x.FirstName + " " + x.LastName;
                        HeaderView.SubText = string.IsNullOrWhiteSpace(name) ? x.Username : name;
                        HeaderView.SetImage(new Avatar(x.Avatar).ToUrl(128), Images.Avatar);
                        RefreshHeaderView();
                    }
                }));
            });

//			if (!ViewModel.IsLoggedInUser)
//				NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
        }
//
//		private void ShowExtraMenu()
//		{
//			var sheet = AlertDialogService.GetSheet(ViewModel.Username);
//			var followButton = sheet.AddButton(ViewModel.IsFollowing ? "Unfollow" : "Follow");
//			var cancelButton = sheet.AddButton("Cancel");
//			sheet.CancelButtonIndex = cancelButton;
//			sheet.DismissWithClickedButtonIndex(cancelButton, true);
//			sheet.Clicked += (s, e) => {
//				if (e.ButtonIndex == followButton)
//				{
//					ViewModel.ToggleFollowingCommand.Execute(null);
//				}
//			};
//
//			sheet.ShowInView(this.View);
//		}
    }
}

