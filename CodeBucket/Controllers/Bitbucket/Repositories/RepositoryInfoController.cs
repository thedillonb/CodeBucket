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

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryInfoController : Controller<RepositoryDetailedModel>, IImageUpdated
    {
        private HeaderView _header;

        public RepositoryInfoController(RepositoryDetailedModel model)
            : base(true)
        {
            Title = model.Name;
            Model = model;
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Refresh();
        }

        protected override void OnRefresh()
        {
            var lastUpdated = "Updated " + (Model.UtcLastUpdated).ToDaysAgo();

            _header = new HeaderView(View.Bounds.Width) { Title = Model.Name, Subtitle = lastUpdated };

            if (!string.IsNullOrEmpty(Model.Logo))
                _header.Image = ImageLoader.DefaultRequestImage(new Uri(Model.Logo), this);

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
                                              Text1 = Model.Scm,
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
                                              Text1 = Model.IsPrivate ? "Private" : "Public",
                                              Image1 = Model.IsPrivate ? Images.Locked : Images.Unlocked,
                                              Text2 = size,
                                              Image2 = Images.Size
                                          }));

            sec1.Add(new SplitElement(new SplitElement.Row
                                          {
                                              Text1 = (Model.UtcCreatedOn).ToString("MM/dd/yy"),
                                              Image1 = Images.Create,
                                              Text2 = Model.ForkCount.ToString() + (Model.ForkCount == 1 ? " Fork" : " Forks"),
                                              Image2 = Images.Fork
                                          }));


            var owner = new StyledElement("Owner", Model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledElement("Followers", "" + Model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(Model.Owner, Model.Slug), true);
            sec1.Add(followers);


            var events = new StyledElement("Events", () => NavigationController.PushViewController(new RepoEventsController(Model.Owner, Model.Slug), true), Images.Event);

            var sec2 = new Section { events };

            if (Model.HasIssues)
                sec2.Add(new StyledElement("Issues", () => NavigationController.PushViewController(new IssuesController(Model.Owner, Model.Slug), true), Images.Flag));

            if (Model.HasWiki)
                sec2.Add(new StyledElement("Wiki", () => NavigationController.PushViewController(new WikiInfoController(Model.Owner, Model.Slug), true), Images.Pencil));

            var sec3 = new Section
                           {
                new StyledElement("Changes", () => NavigationController.PushViewController(new ChangesetController(Model.Owner, Model.Slug), true), Images.Changes),
                new StyledElement("Branches", () => NavigationController.PushViewController(new BranchController(Model.Owner, Model.Slug), true), Images.Branch),
                new StyledElement("Tags", () => NavigationController.PushViewController(new TagController(Model.Owner, Model.Slug), true), Images.Tag)
            };

            Root.Add(new[] { sec1, sec2, sec3 });

            if (!string.IsNullOrEmpty(Model.Website))
            {
                var web = new StyledElement("Website", () => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(Model.Website)), Images.Webpage);
                Root.Add(new Section { web });
            }
        }

        protected override RepositoryDetailedModel OnUpdate(bool forced)
        {
            return Model;
        }

        public void UpdatedImage(Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }

    }
}