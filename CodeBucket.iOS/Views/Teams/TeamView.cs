using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.Elements;
using UIKit;
using CoreGraphics;

namespace CodeBucket.Views.Teams
{
    public class TeamView : PrettyDialogViewController
    {
        public new TeamViewModel ViewModel
        {
            get { return (TeamViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public TeamView()
        {
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.SetImage(null, Images.Avatar);
            Title = ViewModel.Name;

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
                    var members = new StyledStringElement("Members", () => ViewModel.GoToMembersCommand.Execute(null), Images.Team);
                    var midSec = new Section(new UIView(new CGRect(0, 0, 0, 20f))) { events, organizations, members, followers };
                    Root = new RootElement(Title) { midSec, new Section { repos } };
                });
//          if (!ViewModel.IsLoggedInUser)
//              NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
        }
//
//      private void ShowExtraMenu()
//      {
//          var sheet = MonoTouch.Utilities.GetSheet(ViewModel.Username);
//          var followButton = sheet.AddButton(ViewModel.IsFollowing ? "Unfollow" : "Follow");
//          var cancelButton = sheet.AddButton("Cancel");
//          sheet.CancelButtonIndex = cancelButton;
//          sheet.DismissWithClickedButtonIndex(cancelButton, true);
//          sheet.Clicked += (s, e) => {
//              if (e.ButtonIndex == followButton)
//              {
//                  ViewModel.ToggleFollowingCommand.Execute(null);
//              }
//          };
//
//          sheet.ShowInView(this.View);
//      }
    }
}

