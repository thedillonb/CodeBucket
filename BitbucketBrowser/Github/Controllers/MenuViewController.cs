using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers;
using BitbucketBrowser.GitHub.Controllers.Events;
using BitbucketBrowser.GitHub.Controllers.Groups;
using BitbucketBrowser.GitHub.Controllers.Repositories;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketBrowser.Elements;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;
using BitbucketBrowser.GitHub.Controllers.Notifications;
using BitbucketBrowser.GitHub.Controllers.Gists;
using BitbucketBrowser.Controllers;

namespace BitbucketBrowser.GitHub.Controllers
{
    public class MenuController : MenuBaseController
    {
		protected override void OnCreateMenu (RootElement root)
		{
			root.Add(new Section() {
				new MenuElement("Profile", () => NavPush(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person),
				new MenuElement("Events", () => NavPush(new EventsController(Application.Account.Username, false) { ReportRepository = true }), Images.Event),
				new MenuElement("Notifications", () => NavPush(new NotificationsController()), Images.Event),
				new MenuElement("Repositories", () => NavPush(new AccountRepositoryController(Application.Account.Username) { Title = "Repositories" }), Images.Repo),
				new MenuElement("Organizations", () => NavPush(new OrganizationsController(Application.Account.Username, false)), Images.Group),
				new MenuElement("Gists", () => NavPush(new AccountGistsController(Application.Account.Username, false)), Images.Group),
				new MenuElement("Explore", () => NavPush(new ExploreController() { Title = "Explore" }), UIImage.FromBundle("/Images/Tabs/search")),
			});
		}
    }
}
