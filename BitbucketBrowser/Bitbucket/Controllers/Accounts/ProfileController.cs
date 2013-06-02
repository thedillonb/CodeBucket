using CodeBucket.Controllers;
using CodeBucket.Views;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using MonoTouch.Dialog.Utilities;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;
using BitbucketBrowser.Controllers.Followers;
using BitbucketBrowser.Controllers.Events;
using BitbucketBrowser.Controllers.Groups;
using BitbucketBrowser.Controllers.Repositories;

namespace BitbucketBrowser.Controllers
{
	public class ProfileController : Controller<UsersModel>, IImageUpdated
	{
        private HeaderView _header;

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

            var followers = new StyledElement("Followers", () => NavigationController.PushViewController(new UserFollowersController(Username), true), Images.Heart);
            var events = new StyledElement("Events", () => NavigationController.PushViewController(new EventsController(Username) { ReportRepository = true }, true), Images.Event);
            var groups = new StyledElement("Groups", () => NavigationController.PushViewController(new GroupController(Username), true), Images.Group);
            var repos = new StyledElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Username, true) { Model = Model.Repositories, ShowOwner = false }, true), Images.Repo);
            Root.Add(new [] { new Section { followers, events, groups }, new Section { repos } });
        }

        protected override void OnRefresh()
        {
            _header.Subtitle = Model.User.FirstName ?? "" + " " + (Model.User.LastName ?? "");
            _header.Image = ImageLoader.DefaultRequestImage(new System.Uri(Model.User.Avatar), this);
            BeginInvokeOnMainThread(() => _header.SetNeedsDisplay());

            if (Username.Equals(Application.Account.Username, System.StringComparison.OrdinalIgnoreCase))
            {
                Application.Account.AvatarUrl = Model.User.Avatar;
                BeginInvokeOnMainThread(() => Application.Account.Update());
            }
        }

        protected override UsersModel OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].GetInfo(forced);
        }

        public void UpdatedImage (System.Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
	}
}

