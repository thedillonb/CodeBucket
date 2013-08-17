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
using CodeBucket.Views.Accounts;

namespace CodeBucket.Controllers
{
	public class MenuController : MenuBaseController
    {
		protected override void CreateMenuRoot()
		{
            var root = new RootElement(Application.Account.Username);
            root.Add(new Section() {
                new MenuElement("Profile".t(), () => NavPush(new CodeBucket.Views.Accounts.ProfileView(Application.Account.Username) { Title = "Profile".t() }), Images.Buttons.Person),
            });

            var eventsSection = new Section() { HeaderView = new MenuSectionView("Events".t()) };
            eventsSection.Add(new MenuElement(Application.Account.Username, () => NavPush(new EventsController(Application.Account.Username)), Images.Buttons.Event));
            if (Application.Account.Teams != null && !Application.Account.DontShowTeamEvents)
                Application.Account.Teams.ForEach(team => eventsSection.Add(new MenuElement(team, () => NavPush(new EventsController(team)), Images.Buttons.Event)));
            root.Add(eventsSection);

            var repoSection = new Section() { HeaderView = new MenuSectionView("Repositories".t()) };
            repoSection.Add(new MenuElement("Owned".t(), () => NavPush(new RepositoryController(Application.Account.Username) { Title = "Owned".t() }), Images.Repo));
            repoSection.Add(new MenuElement("Following".t(), () => NavPush(new FollowingRepositoryController()), Images.RepoFollow));
            repoSection.Add(new MenuElement("Explore".t(), () => NavPush(new ExploreController()), Images.Buttons.Explore));
            root.Add(repoSection);
            
            var pinnedRepos = Application.Account.GetPinnedRepositories();
            if (pinnedRepos.Count > 0)
            {
                var pinnedRepoSection = new Section() { HeaderView = new MenuSectionView("Favorite Repositories".t()) };
                pinnedRepos.ForEach(x => pinnedRepoSection.Add(new MenuElement(x.Name, () => NavPush(new RepositoryInfoController(x.Owner, x.Slug, x.Name)), Images.Repo) { ImageUri = new System.Uri(x.ImageUri) }));
                root.Add(pinnedRepoSection);
            }

            var groupsTeamsSection = new Section() { HeaderView = new MenuSectionView("Collaborations".t()) };
            if (Application.Account.DontExpandTeamsAndGroups)
            {
                groupsTeamsSection.Add(new MenuElement("Groups".t(), () => NavPush(new GroupController(Application.Account.Username)), Images.Buttons.Group));
                groupsTeamsSection.Add(new MenuElement("Teams".t(), () => NavPush(new TeamController()), Images.Team));
            }
            else
            {
                if (Application.Account.Groups != null)
                    Application.Account.Groups.ForEach(x => groupsTeamsSection.Add(new MenuElement(x.Name, () => NavPush(new GroupMembersController(Application.Account.Username, x.Slug) { Title = x.Name, Model = x.Members }), Images.Buttons.Group)));
                if (Application.Account.Teams != null)
                    Application.Account.Teams.ForEach(x => groupsTeamsSection.Add(new MenuElement(x, () => NavPush(new ProfileView(x)), Images.Team)));
            }

            //There should be atleast 1 thing...
            if (groupsTeamsSection.Elements.Count > 0)
                root.Add(groupsTeamsSection);

            var infoSection = new Section() { HeaderView = new MenuSectionView("Info & Preferences".t()) };
            root.Add(infoSection);
            infoSection.Add(new MenuElement("Settings".t(), () => NavPush(new SettingsController()), Images.Buttons.Cog));
            infoSection.Add(new MenuElement("About".t(), () => NavPush(new AboutController()), Images.Buttons.Info));
            infoSection.Add(new MenuElement("Feedback & Support".t(), PresentUserVoice, Images.Buttons.Flag));
            infoSection.Add(new MenuElement("Accounts".t(), () => ProfileButtonClicked(this, System.EventArgs.Empty), Images.Buttons.User));
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

