using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using BitbucketSharp.Models;
using System.Threading;
using BitbucketSharp;

namespace BitbucketBrowser.UI
{
	public class ProfileController : Controller<UsersModel>
	{
        public string Username { get; private set; }

		public ProfileController(string username, bool push = true) 
            : base(push)
		{
            Title = username;
			Username = username;
		}

        protected override void OnRefresh()
        {
            InvokeOnMainThread(delegate 
            {
                Root.Clear();
                Root.Add(new Section(new HeaderView(View.Bounds.Width) { 
                    Title = Model.User.Username, Subtitle = Model.User.FirstName ?? "" + " " + Model.User.LastName ?? "" 
                }));
                
                var repoSec = new Section() {
                    new ImageStringElement("Followers", () =>  NavigationController.PushViewController(new UserFollowersController(Model.User.Username), true),
                                           UIImage.FromBundle("Images/heart.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }, 
                    new ImageStringElement("Events", () => NavigationController.PushViewController(new EventsController(Model.User.Username), true),
                                           UIImage.FromBundle("Images/repoevents.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                    new ImageStringElement("Groups", () => NavigationController.PushViewController(new GroupController(Model.User.Username), true),
                                           UIImage.FromBundle("Images/followers.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                };
                Root.Add(repoSec);
    
                var sec2 = new Section() {
                    new ImageStringElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Model.User.Username) { Model = Model.Repositories }, true),
                                           UIImage.FromBundle("Images/repo.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                };
                Root.Add(sec2);
             });
        }

        protected override UsersModel OnUpdate()
        {
            var client = new Client("thedillonb", "djames");
            return client.Users[Username].GetInfo();
        }
	}
}

