using BitbucketBrowser.GitHub.Controllers.Events;
using BitbucketBrowser.GitHub.Controllers.Followers;
using BitbucketBrowser.GitHub.Controllers.Groups;
using BitbucketBrowser.GitHub.Controllers.Repositories;
using MonoTouch.Dialog;
using GitHubSharp.Models;
using MonoTouch.Dialog.Utilities;
using BitbucketBrowser.Controllers;
using CodeFramework.UI.Views;
using BitbucketBrowser.Elements;
using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers.Gists;

namespace BitbucketBrowser.GitHub.Controllers
{
    public class ProfileController : Controller<UserModel>, IImageUpdated
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
            var groups = new StyledElement("Organizations", () => NavigationController.PushViewController(new OrganizationsController(Username), true), Images.Group);
            var repos = new StyledElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Username) { ShowOwner = false }, true), Images.Repo);
            var gists = new StyledElement("Gists", () => NavigationController.PushViewController(new GistsController(Username), true), Images.Repo);

            Root.Add(new [] { new Section { followers, events, groups }, new Section { repos, gists} });
        }

        protected override void OnRefresh()
        {
            _header.Subtitle = Model.Name;
            _header.Image = ImageLoader.DefaultRequestImage(new System.Uri(Model.AvatarUrl), this);
            BeginInvokeOnMainThread(() => _header.SetNeedsDisplay());
        }

        protected override UserModel OnUpdate(bool forced)
        {
            return Application.GitHubClient.API.GetUser(Username).Data;
        }

        public void UpdatedImage (System.Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
	}
}


