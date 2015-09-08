using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Elements;
using UIKit;
using CoreGraphics;

namespace CodeBucket.Views.User
{
    public class ProfileView : PrettyDialogViewController
    {
		public new ProfileViewModel ViewModel
		{
			get { return (ProfileViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public ProfileView()
		{
			Root.UnevenRows = true;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.SetImage(null, Images.Avatar);
            HeaderView.Text = ViewModel.Username;
            Title = ViewModel.Username;

			ViewModel.Bind(x => x.User, x =>
			{
                var name = x.FirstName + " " + x.LastName;
                HeaderView.SubText = string.IsNullOrWhiteSpace(name) ? x.Username : name;
                HeaderView.SetImage(x.Avatar, Images.RepoPlaceholder);
                RefreshHeaderView();

				var followers = new StyledStringElement("Followers", () => ViewModel.GoToFollowersCommand.Execute(null), Images.Heart);
				var events = new StyledStringElement("Events", () => ViewModel.GoToEventsCommand.Execute(null), Images.Event);
                var organizations = new StyledStringElement("Groups", () => ViewModel.GoToGroupsCommand.Execute(null), Images.Group);
				var repos = new StyledStringElement("Repositories", () => ViewModel.GoToRepositoriesCommand.Execute(null), Images.Repo);
                var following = new StyledStringElement("Following", () => ViewModel.GoToFollowingCommand.Execute(null), Images.Following);
                var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, followers, following };
				Root = new RootElement(Title) { midSec, new Section { repos } };
			});
//			if (!ViewModel.IsLoggedInUser)
//				NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
        }
//
//		private void ShowExtraMenu()
//		{
//			var sheet = MonoTouch.Utilities.GetSheet(ViewModel.Username);
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

