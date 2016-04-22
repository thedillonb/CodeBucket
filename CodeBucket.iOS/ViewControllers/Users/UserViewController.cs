using CodeBucket.Core.ViewModels.Users;
using CodeBucket.DialogElements;
using UIKit;
using CoreGraphics;
using CodeBucket.Core.Utils;
using System;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Users
{
    public class UserViewController : PrettyDialogViewController
    {
		public new UserViewModel ViewModel
		{
			get { return (UserViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.Text = ViewModel.Username;
            Title = ViewModel.Username;

            var followers = new StringElement("Followers", AtlassianIcon.Star.ToImage());
            followers.BindClick(ViewModel.GoToFollowersCommand);

            var events = new StringElement("Events", AtlassianIcon.Blogroll.ToImage());
            events.BindClick(ViewModel.GoToEventsCommand);

            var groups = new StringElement("Groups", AtlassianIcon.Group.ToImage()) { Hidden = true };
            groups.BindClick(ViewModel.GoToGroupsCommand);

            var repos = new StringElement("Repositories", AtlassianIcon.Devtoolsrepository.ToImage());
            repos.Clicked.BindCommand(ViewModel.GoToRepositoriesCommand);

            var following = new StringElement("Following", AtlassianIcon.View.ToImage());
            following.Clicked.BindCommand(ViewModel.GoToFollowingCommand);

            var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, groups, followers, following };
            Root.Reset(midSec, new Section { repos });

            ViewModel.Bind(x => x.User, true).Subscribe(x => {
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

            ViewModel.Bind(x => x.ShouldShowGroups, true)
                     .Subscribe(x => groups.Hidden = !x);

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

