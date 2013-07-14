using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;
using CodeBucket.Bitbucket.Controllers.Events;
using CodeBucket.Bitbucket.Controllers.Repositories;
using CodeBucket.Bitbucket.Controllers.Groups;
using System.Threading;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeBucket.Bitbucket.Controllers.Teams;

namespace CodeBucket.Bitbucket.Controllers
{
	public class MenuController : MenuBaseController
    {
		protected override void CreateMenu(RootElement root)
		{
            root.Add(new Section() {
                new MenuElement("Profile", () => NavPush(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person),
            });

            var eventsSection = new Section() { HeaderView = new MenuSectionView("Events") };
            eventsSection.Add(new MenuElement(Application.Account.Username, () => NavPush(new EventsController(Application.Account.Username, false)), Images.Event));
            if (Application.Account.Teams != null && !Application.Account.DontShowTeamEvents)
            {
                foreach (var t in Application.Account.Teams)
                {
                    var team = t; //Capture
                    eventsSection.Add(new MenuElement(team.DisplayName, () => NavPush(new EventsController(team.Username, false)), Images.Event));
                }
            }
            root.Add(eventsSection);

            var repoSection = new Section() { HeaderView = new MenuSectionView("Repositories") };
            repoSection.Add(new MenuElement("Owned", () => NavPush(new RepositoryController(Application.Account.Username, false) { Title = "Owned" }), Images.Repo));
            repoSection.Add(new MenuElement("Following", () => NavPush(new FollowingRepositoryController()), Images.RepoFollow));
            repoSection.Add(new MenuElement("Explore", () => NavPush(new ExploreController()), UIImage.FromBundle("/Images/Tabs/search")));
            root.Add(repoSection);

            var groupsTeamsSection = new Section() { HeaderView = new MenuSectionView("Groups & Teams") };
            groupsTeamsSection.Add(new MenuElement("Groups", () => NavPush(new GroupController(Application.Account.Username, false)), Images.Group));
            groupsTeamsSection.Add(new MenuElement("Teams", () => NavPush(new TeamController(false)), Images.Team));
            root.Add(groupsTeamsSection);

            var settingsSection = new Section() { HeaderView = new MenuSectionView("Settings") };
            root.Add(settingsSection);
            settingsSection.Add(new MenuElement("Accounts", () => ProfileButtonClicked(this, System.EventArgs.Empty), null));
            settingsSection.Add(new MenuElement("Settings", () => NavPush(new SettingsController()), null));

            var infoSection = new Section() { HeaderView = new MenuSectionView("Info") };
            root.Add(infoSection);
            infoSection.Add(new MenuElement("About", () => NavPush(new AboutController()), null));
            infoSection.Add(new MenuElement("Feedback & Support", PresentUserVoice, null));
		}

        private void PresentUserVoice()
        {
            var config = UserVoice.UVConfig.Create("http://codebucket.uservoice.com", "pnuDmPENErDiDpXrms1DTg", "iDboMdCIwe2E5hJFa8hy9K9I5wZqnjKCE0RPHLhZIk");
            UserVoice.UserVoice.PresentUserVoiceInterface(this, config);
        }

        protected override void ProfileButtonClicked(object sender, System.EventArgs e)
        {
            var accounts = new AccountsController();
            PresentViewController(new UINavigationController(accounts), true, null);
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.Account.Username;
            ProfileButton.Uri = new System.Uri(Application.Account.AvatarUrl);

            //This must be last.
            base.ViewWillAppear(animated);
        }
    }
}

