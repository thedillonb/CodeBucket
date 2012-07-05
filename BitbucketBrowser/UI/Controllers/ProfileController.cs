using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using BitbucketSharp.Models;
using System.Threading;
using BitbucketSharp;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{
	public class ProfileController : Controller<UsersModel>
	{
        private HeaderView _header;
        private StyledElement _followers;
        private StyledElement _events;
        private StyledElement _groups;
        private StyledElement _repos;

        public string Username { get; private set; }

		public ProfileController(string username, bool push = true) 
            : base(push)
		{
            Title = username;
			Username = username;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _header = new HeaderView(View.Bounds.Width) { Title = Username };
            Root.Add(new Section(_header));

            _followers = new StyledElement("Followers", () => NavigationController.PushViewController(new UserFollowersController(Model.User.Username), true),
                                                UIImage.FromBundle("Images/heart.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            _events = new StyledElement("Events", () => NavigationController.PushViewController(new EventsController(Model.User.Username) { ReportUser = false, ReportRepository = true }, true), 
                                             UIImage.FromBundle("Images/repoevents.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            _groups = new StyledElement("Groups", () => NavigationController.PushViewController(new GroupController(Model.User.Username), true), 
                                             UIImage.FromBundle("Images/followers.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };

            _repos = new StyledElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Model.User.Username) { Model = Model.Repositories }, true), 
                                            UIImage.FromBundle("Images/repo.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            Root.Add(new [] { new Section { _followers, _events, _groups }, new Section { _repos } });
        }

        protected override void OnRefresh()
        {
            _header.Subtitle = Model.User.FirstName ?? "" + " " + Model.User.LastName ?? "";

            NSUrl url = new NSUrl(Model.User.Avatar);
            var data = NSData.FromUrl(url);
            _header.Image = new UIImage(data);
            InvokeOnMainThread(delegate { _header.SetNeedsDisplay(); });
        }

        protected override UsersModel OnUpdate()
        {
            return Application.Client.Users[Username].GetInfo();
        }
	}
}

