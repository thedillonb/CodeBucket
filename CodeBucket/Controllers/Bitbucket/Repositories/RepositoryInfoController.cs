using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Bitbucket.Controllers.Issues;
using CodeBucket.Bitbucket.Controllers.Followers;
using CodeBucket.Bitbucket.Controllers.Events;
using CodeBucket.Bitbucket.Controllers.Wikis;
using CodeBucket.Bitbucket.Controllers.Branches;
using CodeBucket.Bitbucket.Controllers.Changesets;
using CodeBucket.Controllers;
using CodeFramework.Views;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryInfoController : BaseModelDrivenController, IImageUpdated
    {
        private HeaderView _header;

        public new RepositoryDetailedModel Model 
        { 
            get { return (RepositoryDetailedModel)base.Model; } 
            set { base.Model = value; }
        }

        public string Username { get; private set; }

        public string Repo { get; private set; }

        public RepositoryInfoController(string username, string repo)
            : base(typeof(RepositoryDetailedModel))
        {
            Username = username;
            Repo = repo;
            Title = repo;

            _header = new HeaderView(View.Bounds.Width) { Title = repo, ShadowImage = false };
        }

        public RepositoryInfoController(RepositoryDetailedModel model)
            : this(model.Owner, model.Name)
        {
            Model = model;
        }

        protected override void OnRender()
        {
            var model = Model;
            var root = new RootElement(Title) { UnevenRows = true };
            _header.Subtitle = "Updated " + (model.UtcLastUpdated).ToDaysAgo();

            if (!string.IsNullOrEmpty(model.Logo))
                _header.Image = ImageLoader.DefaultRequestImage(new Uri(model.LargeLogo(64)), this);

            root.Add(new Section(_header));
            var sec1 = new Section();

            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description))
            {
                var element = new MultilinedElement(model.Description)
                {
                    BackgroundColor = UIColor.White
                };
                element.CaptionColor = element.ValueColor;
                element.CaptionFont = element.ValueFont;
                sec1.Add(element);
            }

            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = model.Scm,
                Image1 = Images.ScmType,
                Text2 = model.Language,
                Image2 = Images.Language
            }));


            //Calculate the best representation of the size
            string size;
            if (model.Size / 1024f < 1)
                size = string.Format("{0}B", model.Size);
            else if ((model.Size / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}KB", model.Size / 1024f);
            else if ((model.Size / 1024f / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}MB", model.Size / 1024f / 1024f);
            else
                size = string.Format("{0:0.##}GB", model.Size / 1024f / 1024f / 1024f);


            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = model.IsPrivate ? "Private" : "Public",
                Image1 = model.IsPrivate ? Images.Locked : Images.Unlocked,
                Text2 = size,
                Image2 = Images.Size
            }));

            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = (model.UtcCreatedOn).ToString("MM/dd/yy"),
                Image1 = Images.Create,
                Text2 = model.ForkCount.ToString() + (model.ForkCount == 1 ? " Fork" : " Forks"),
                Image2 = Images.Fork
            }));


            var owner = new StyledElement("Owner", model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledElement("Followers", "" + model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(model.Owner, model.Slug), true);
            sec1.Add(followers);


            var events = new StyledElement("Events", () => NavigationController.PushViewController(new RepoEventsController(model.Owner, model.Slug), true), Images.Buttons.Event);

            var sec2 = new Section { events };

            if (model.HasIssues)
                sec2.Add(new StyledElement("Issues", () => NavigationController.PushViewController(new IssuesController(model.Owner, model.Slug), true), Images.Buttons.Flag));

            if (model.HasWiki)
                sec2.Add(new StyledElement("Wiki", () => NavigationController.PushViewController(new WikiInfoController(model.Owner, model.Slug), true), Images.Pencil));

            var sec3 = new Section
            {
                new StyledElement("Changes", () => NavigationController.PushViewController(new ChangesetController(model.Owner, model.Slug), true), Images.Changes),
                new StyledElement("Branches", () => NavigationController.PushViewController(new BranchController(model.Owner, model.Slug), true), Images.Branch),
                new StyledElement("Tags", () => NavigationController.PushViewController(new TagController(model.Owner, model.Slug), true), Images.Tag)
            };

            root.Add(new[] { sec1, sec2, sec3 });

            if (!string.IsNullOrEmpty(model.Website))
            {
                var web = new StyledElement("Website", () => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(model.Website)), Images.Webpage);
                root.Add(new Section { web });
            }

            Root = root;
        }

        protected override object OnUpdateModel(bool forced)
        {
            return Application.Client.Users[Username].Repositories[Repo].GetInfo(forced);
        }

        public void UpdatedImage(Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }

    }
}