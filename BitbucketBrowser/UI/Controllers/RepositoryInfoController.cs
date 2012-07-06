using System.Drawing;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using BitbucketBrowser.Utils;

namespace BitbucketBrowser.UI
{
    public class RepositoryInfoController : Controller<RepositoryDetailedModel>
    {

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
            var lastUpdated = "Updated " + DateTime.Parse(Model.UtcLastUpdated).ToDaysAgo();

            var header = new HeaderView(View.Bounds.Width) { 
                Title = Model.Name, Subtitle = lastUpdated
            };

            if (!string.IsNullOrEmpty(Model.Logo))
            {
                var url = new NSUrl(Model.Logo);
                var data = NSData.FromUrl(url);
                header.Image = new UIImage(data);
            }

            Root.Add(new Section(header));
            var sec1 = new Section();


            
            if (!string.IsNullOrEmpty(Model.Description) && !string.IsNullOrWhiteSpace(Model.Description))
            {
                sec1.Add(new MultilineElement(Model.Description));
            }

            sec1.Add(new RepositoryInfo(new [] { new RepositoryInfo.Row() { Text1 = Model.Scm, Image1 = Images.ScmType,
                Text2 = Model.Language, Image2 = Images.Language } }));



            //Calculate the best representation of the size
            string size = "";
            if (Model.Size / 1024f < 1)
                size = string.Format("{0}B", Model.Size);
            else if ((Model.Size / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}KB", Model.Size / 1024f);
            else
                size = string.Format("{0:0.##}MB", Model.Size / 1024f / 1024f);


            sec1.Add(new RepositoryInfo(new [] { 
                new RepositoryInfo.Row() { 
                    Text1 = Model.IsPrivate ? "Private" : "Public" , Image1 = Model.IsPrivate ? Images.Locked : Images.Unlocked,
                    Text2 = size, Image2 = Images.Size } }));

            sec1.Add(new RepositoryInfo(new [] { new RepositoryInfo.Row() { Text1 = DateTime.Parse(Model.CreatedOn).ToString("MM.dd.yy"), Image1 = UIImage.FromBundle("/Images/create"),
                Text2 = DateTime.Parse(Model.UtcLastUpdated).ToString("MM.dd.yy"), Image2 = UIImage.FromBundle("/Images/pencil") } }));


            var owner = new StyledElement("Owner", Model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledElement("Followers", "" + Model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(Model.Owner, Model.Slug), true);
            sec1.Add(followers);


            var events = new StyledElement("Events", UIImage.FromBundle("Images/repoevents.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            events.Tapped += () => NavigationController.PushViewController(new RepoEventsController(Model.Owner, Model.Slug), true);

            var sec2 = new Section();
            sec2.Add(events);

            if (Model.HasIssues) 
                sec2.Add(new StyledElement("Issues", () => NavigationController.PushViewController(new IssuesController(Model.Owner, Model.Slug), true),
                                                UIImage.FromBundle("Images/flag")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
                );

            if (Model.HasWiki)
                sec2.Add(new StyledElement("Wiki", () => NavigationController.PushViewController(new WikiInfoController(Model.Owner, Model.Slug), true),
                                                UIImage.FromBundle("Images/pencil.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
                );

            var sec3 = new Section() {
                new StyledElement("Changes", () => NavigationController.PushViewController(new ChangesetController(Model.Owner, Model.Slug), true), 
                                       UIImage.FromBundle("Images/commit.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                new StyledElement("Branches", () => NavigationController.PushViewController(new BranchController(Model.Owner, Model.Slug), true),
                                       UIImage.FromBundle("Images/branch.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                new StyledElement("Tags", () => NavigationController.PushViewController(new TagController(Model.Owner, Model.Slug), true),
                                        UIImage.FromBundle("Images/tag.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
            };


            
            Root.Add(new [] { sec1, sec2, sec3 });

            if (!string.IsNullOrEmpty(Model.Website))
            {
                var web = new StyledElement("Website", () => {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(Model.Website));
                }, Images.Webpage) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
                Root.Add(new Section() { web });
            }
        }

        protected override RepositoryDetailedModel OnUpdate()
        {
            return Model;
        }
    }
}