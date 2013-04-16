using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers.Branches;
using BitbucketBrowser.GitHub.Controllers.Changesets;
using BitbucketBrowser.GitHub.Controllers.Events;
using BitbucketBrowser.GitHub.Controllers.Followers;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using BitbucketBrowser.Elements;
using BitbucketBrowser.Controllers;
using CodeFramework.UI.Views;
using BitbucketBrowser.GitHub.Controllers.Readme;

namespace BitbucketBrowser.GitHub.Controllers.Repositories
{
    public class RepositoryInfoController : Controller<RepositoryModel>, IImageUpdated
    {
        private HeaderView _header;
        private readonly string _user;
        private readonly string _repo;

        public RepositoryInfoController(string user, string repo)
            : base(true)
        {
            _user = user;
            _repo = repo;
            Title = repo;
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _header = new HeaderView(View.Bounds.Width) { Title = _repo };
        }

        protected override void OnRefresh()
        {
            InvokeOnMainThread(DoRefresh);
        }

        private void DoRefresh()
        {
            _header.Subtitle = "Updated " + Model.UpdatedAt.ToDaysAgo();
            _header.SetNeedsDisplay();

//            if (!string.IsNullOrEmpty(Model.Logo))
//                _header.Image = ImageLoader.DefaultRequestImage(new Uri(Model.Logo), this);

            Root.Add(new Section(_header));
            var sec1 = new Section();

            if (!string.IsNullOrEmpty(Model.Description) && !string.IsNullOrWhiteSpace(Model.Description))
            {
                var element = new MultilinedElement(Model.Description)
                {
                    BackgroundColor = UIColor.White
                };
                element.CaptionColor = element.ValueColor;
                element.CaptionFont = element.ValueFont;
                sec1.Add(element);
            }

            sec1.Add(new SplitElement(new SplitElement.Row
                                          {
                                              Text1 = "git",
                                              Image1 = Images.ScmType,
                                              Text2 = Model.Language,
                                              Image2 = Images.Language
                                          }));


            //Calculate the best representation of the size
            string size;
            if (Model.Size / 1024f < 1)
                size = string.Format("{0}B", Model.Size);
            else if ((Model.Size / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}KB", Model.Size / 1024f);
            else if ((Model.Size / 1024f / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}MB", Model.Size / 1024f / 1024f);
            else
                size = string.Format("{0:0.##}GB", Model.Size / 1024f / 1024f / 1024f);


            sec1.Add(new SplitElement(new SplitElement.Row
                                          {
                                              Text1 = Model.Private ? "Private" : "Public",
                                              Image1 = Model.Private ? Images.Locked : Images.Unlocked,
                                              Text2 = size,
                                              Image2 = Images.Size
                                          }));

            sec1.Add(new SplitElement(new SplitElement.Row
                                          {
                                              Text1 = Model.CreatedAt.ToString("MM/dd/yy"),
                                              Image1 = Images.Create,
                                              Text2 = Model.Forks.ToString() + (Model.Forks == 1 ? " Fork" : " Forks"),
                                              Image2 = Images.Fork
                                          }));


            var owner = new StyledElement("Owner", Model.Owner.Login) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Owner.Login), true);
            sec1.Add(owner);
            var followers = new StyledElement("Watchers", "" + Model.Watchers) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(_user, _repo), true);
            sec1.Add(followers);


            var events = new StyledElement("Events", () => NavigationController.PushViewController(new RepoEventsController(_user, _repo), true), Images.Event);

            var sec2 = new Section { events };

//            if (Model.HasIssues)
//                sec2.Add(new StyledElement("Issues", () => NavigationController.PushViewController(new IssuesController(_user, _repo), true), Images.Flag));

            if (Model.HasWiki)
                sec2.Add(new StyledElement("Wiki", () => NavigationController.PushViewController(new ReadmeController(_user, _repo), true), Images.Pencil));

            var sec3 = new Section
                           {
                new StyledElement("Changes", () => NavigationController.PushViewController(new ChangesetController(_user, _repo), true), Images.Changes),
                new StyledElement("Branches", () => NavigationController.PushViewController(new BranchController(_user, _repo), true), Images.Branch),
                new StyledElement("Tags", () => NavigationController.PushViewController(new TagController(_user, _repo), true), Images.Tag)
            };

            Root.Add(new[] { sec1, sec2, sec3 });

            if (!string.IsNullOrEmpty(Model.Homepage))
            {
                var web = new StyledElement("Website", () => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(Model.Homepage)), Images.Webpage);
                Root.Add(new Section { web });
            }
        }

        protected override RepositoryModel OnUpdate(bool forced)
        {
            return Application.GitHubClient.API.GetRepository(_user, _repo).Data;
        }

        public void UpdatedImage(Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }

    }
}