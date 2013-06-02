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

namespace CodeBucket.Bitbucket.Controllers
{
	public class MenuController : MenuBaseController
    {
		protected override void OnCreateMenu(RootElement root)
		{
			root.Add(new Section() {
				new MenuElement("Profile", () => NavPush(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person),
				new MenuElement("Events", () => NavPush(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true }), Images.Event),
				new MenuElement("Repositories", () => NavPush(new AccountRepositoryController(Application.Account.Username) { Title = "Repositories" }), Images.Repo),
				new MenuElement("Groups", () => NavPush(new GroupController(Application.Account.Username, false) { Title = "Groups" }), Images.Group),
				new MenuElement("Explore", () => NavPush(new ExploreController() { Title = "Explore" }), UIImage.FromBundle("/Images/Tabs/search")),
			});
		}
    }
}

