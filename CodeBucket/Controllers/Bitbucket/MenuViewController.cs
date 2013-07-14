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

namespace CodeBucket.Bitbucket.Controllers
{
	public class MenuController : MenuBaseController
    {
		protected override void CreateMenu(RootElement root)
		{
            var bitbucketSection = new Section() { HeaderView = new MenuSectionView("Bitbucket") };
            root.Add(bitbucketSection);
            bitbucketSection.Add(new MenuElement("Profile", () => NavPush(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person));
            bitbucketSection.Add(new MenuElement("Events", () => NavPush(new EventsController(Application.Account.Username, false) { ReportRepository = true }), Images.Event));
            bitbucketSection.Add(new MenuElement("Repositories", () => NavPush(new AccountRepositoryController(Application.Account.Username) { Title = "Repositories" }), Images.Repo));
            bitbucketSection.Add(new MenuElement("Groups", () => NavPush(new GroupController(Application.Account.Username, false)), Images.Group));
            bitbucketSection.Add(new MenuElement("Explore", () => NavPush(new ExploreController()), UIImage.FromBundle("/Images/Tabs/search")));

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

