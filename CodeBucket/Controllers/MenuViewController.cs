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
using CodeFramework.Utils;
using System.Collections.Generic;

namespace CodeBucket.Controllers
{
	public class MenuController : MenuBaseController
    {
		protected override void CreateMenuRoot()
		{
            var root = new RootElement(Application.Account.Username);
            root.Add(new Section() {
                new MenuElement("Profile", () => NavPush(new ProfileController(Application.Account.Username) { Title = "Profile" }), Images.Buttons.Person),
            });

            var eventsSection = new Section() { HeaderView = new MenuSectionView("Events") };
            eventsSection.Add(new MenuElement(Application.Account.Username, () => NavPush(new EventsController(Application.Account.Username)), Images.Buttons.Event));
            if (Application.Account.Teams != null && !Application.Account.DontShowTeamEvents)
                Application.Account.Teams.ForEach(team => eventsSection.Add(new MenuElement(team, () => NavPush(new EventsController(team)), Images.Buttons.Event)));
            root.Add(eventsSection);

            var repoSection = new Section() { HeaderView = new MenuSectionView("Repositories") };
            repoSection.Add(new MenuElement("Owned", () => NavPush(new RepositoryController(Application.Account.Username, false) { Title = "Owned" }), Images.Repo));
            repoSection.Add(new MenuElement("Following", () => NavPush(new FollowingRepositoryController()), Images.RepoFollow));
            repoSection.Add(new MenuElement("Explore", () => NavPush(new ExploreController()), Images.Buttons.Explore));
            root.Add(repoSection);

            var groupsTeamsSection = new Section() { HeaderView = new MenuSectionView("Collaborations") };
            if (Application.Account.DontExpandTeamsAndGroups)
            {
                groupsTeamsSection.Add(new MenuElement("Groups", () => NavPush(new GroupController(Application.Account.Username)), Images.Buttons.Group));
                groupsTeamsSection.Add(new MenuElement("Teams", () => NavPush(new TeamController()), Images.Team));
            }
            else
            {
                if (Application.Account.Groups != null)
                    Application.Account.Groups.ForEach(x => groupsTeamsSection.Add(new MenuElement(x.Name, () => NavPush(new GroupMembersController(Application.Account.Username, x.Slug) { Title = x.Name, Model = x.Members }), Images.Buttons.Group)));
                if (Application.Account.Teams != null)
                    Application.Account.Teams.ForEach(x => groupsTeamsSection.Add(new MenuElement(x, () => NavPush(new ProfileController(x)), Images.Team)));
            }

            //There should be atleast 1 thing...
            if (groupsTeamsSection.Elements.Count > 0)
                root.Add(groupsTeamsSection);

            var infoSection = new Section() { HeaderView = new MenuSectionView("Info & Preferences") };
            root.Add(infoSection);
            infoSection.Add(new MenuElement("Settings", () => NavPush(new SettingsController()), Images.Buttons.Cog));
            infoSection.Add(new MenuElement("About", () => NavPush(new AboutController()), Images.Buttons.Info));
            infoSection.Add(new MenuElement("Feedback & Support", PresentUserVoice, Images.Buttons.Flag));
            infoSection.Add(new MenuElement("Accounts", () => ProfileButtonClicked(this, System.EventArgs.Empty), Images.Buttons.User));
            Root = root;
		}

        private void PresentUserVoice()
        {
            var config = UserVoice.UVConfig.Create("http://codebucket.uservoice.com", "pnuDmPENErDiDpXrms1DTg", "iDboMdCIwe2E5hJFa8hy9K9I5wZqnjKCE0RPHLhZIk");
            UserVoice.UserVoice.PresentUserVoiceInterface(this, config);
        }

        protected override void ProfileButtonClicked(object sender, System.EventArgs e)
        {
            var accounts = new AccountsController();
            var nav = new UINavigationController(accounts);
            accounts.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Cancel, () => {
                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                Transitions.Transition(appDelegate.Slideout, UIViewAnimationOptions.TransitionFlipFromRight);
            }));
            Transitions.Transition(nav, UIViewAnimationOptions.TransitionFlipFromLeft);
        }

        public override void ViewDidLoad()
        {
            ProfileButton.Uri = new System.Uri(Application.Account.AvatarUrl);

            //Must be in the middle
            base.ViewDidLoad();

            //Load optional stuff
            LoadExtras();
        }

        private void LoadExtras()
        {
            this.DoWorkNoHud(() => {
                var privileges = Application.Client.Account.GetPrivileges();
                Application.Account.Groups = Application.Client.Account.Groups.GetGroups();

                if (privileges != null && privileges.Teams != null)
                {
                    Application.Account.Teams = privileges.Teams.Keys.ToList();
                    Application.Account.Teams.Remove(Application.Account.Username);
                }

                BeginInvokeOnMainThread(() => CreateMenuRoot());
            });
        }
    }
}

