using CodeBucket.Controllers;
using GitHubSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Dialog.Utilities;
using CodeBucket.Views;
using System;
using CodeBucket.Elements;
using CodeBucket.GitHub.Controllers.Followers;
using CodeBucket.GitHub.Controllers.Gists;
using CodeBucket.GitHub.Controllers.Repositories;
using MonoTouch.Foundation;

namespace CodeBucket.GitHub.Controllers.Organizations
{
    public class OrganizationInfoController : Controller<UserModel>, IImageUpdated
    {
        private HeaderView _header;
        public string Org { get; private set; }
        
        public OrganizationInfoController(string org)
            : base(true)
        {
            Org = org;
            Title = org;
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _header = new HeaderView(View.Bounds.Width) { Title = Org };
        }

        protected override void OnRefresh()
        {
            InvokeOnMainThread(DoRefresh);
        }
        
        private void DoRefresh()
        {
            _header.Subtitle = Model.Company;
            if (!string.IsNullOrEmpty(Model.AvatarUrl))
                _header.Image = ImageLoader.DefaultRequestImage(new Uri(Model.AvatarUrl), this);
            _header.SetNeedsDisplay();

            var sec = new Section();
            var sec2 = new Section();
            var root = new RootElement(Title) { new Section(_header), sec, sec2 };

            var followers = new StyledElement("Followers", () => NavigationController.PushViewController(new UserFollowersController(Org), true), Images.Heart);
            sec.Add(followers);

            //var events = new StyledElement("Events", () => NavigationController.PushViewController(new EventsController(Username) { ReportRepository = true }, true), Images.Event);

            var gists = new StyledElement("Gists", () => NavigationController.PushViewController(new GistsController(Org), true), Images.Repo );
            sec.Add(gists);

            var repos = new StyledElement("Repositories", () => NavigationController.PushViewController(new RepositoryController(Org) { ShowOwner = false }, true), Images.Repo);
            sec.Add(repos);

            if (!String.IsNullOrEmpty(Model.Blog))
            {
                var blog = new StyledElement("Blog", () => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(Model.Blog)), Images.Webpage);
                sec2.Add(blog);
            }

            Root = root;
        }

        protected override UserModel OnUpdate(bool forced)
        {
            return Application.GitHubClient.API.GetOrganization(Org).Data;
        }

        public void UpdatedImage(Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
    }
}
