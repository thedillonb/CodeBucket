using CodeBucket.GitHub.Controllers.Events;
using CodeBucket.GitHub.Controllers.Followers;
using CodeBucket.GitHub.Controllers.Repositories;
using MonoTouch.Dialog;
using GitHubSharp.Models;
using MonoTouch.Dialog.Utilities;
using CodeBucket.GitHub.Controllers.Gists;
using CodeBucket.Controllers;
using CodeBucket.Views;
using CodeBucket.Elements;
using CodeBucket.GitHub.Controllers.Organizations;

namespace CodeBucket.GitHub.Controllers.Accounts
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
            var gists = new StyledElement("Gists", () => NavigationController.PushViewController(new GistsController(Username), true), Images.Script);

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


