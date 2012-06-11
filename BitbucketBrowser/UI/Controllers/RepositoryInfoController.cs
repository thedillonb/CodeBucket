using System.Drawing;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{
    public class RepositoryInfoController : Controller<RepositoryDetailedModel>
    {

        public RepositoryInfoController(RepositoryDetailedModel model)
            : base(true)
        {
            Title = model.Name;
            Model = model;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Refresh();
        }

        protected override void OnRefresh()
        {
            var dt = DateTime.Now - DateTime.Parse(Model.LastUpdated);
            var lastUpdated = "Last updated: ";
            if (dt.TotalDays > 1)
                lastUpdated += Convert.ToInt32(dt.TotalDays) + " days ago";
            else if (dt.TotalHours > 1)
                lastUpdated += Convert.ToInt32(dt.TotalHours) + " hours ago";
            else if (dt.TotalMinutes > 1)
                lastUpdated += Convert.ToInt32(dt.TotalMinutes) + " hours ago";
            else
                lastUpdated += "moments ago";

            var header = new HeaderView(View.Bounds.Width) { 
                Title = Model.Name, Subtitle = lastUpdated
            };

            if (!string.IsNullOrEmpty(Model.Logo)) {
                var url = new NSUrl(Model.Logo);
                var data = NSData.FromUrl(url);
                header.Image = new UIImage(data);
            }

            Root.Add(new Section(header));
            var sec1 = new Section();
            
            if (!string.IsNullOrEmpty(Model.Description) && !string.IsNullOrWhiteSpace(Model.Description))
            {
                sec1.Add(new StyledMultilineElement(Model.Description) {
                     Lines = 4,
                     LineBreakMode = UILineBreakMode.WordWrap,
                     Font = UIFont.SystemFontOfSize(14f)
                }
                );
            }

            var ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
            
            var owner = new StyledStringElement("Owner", Model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => ad.ExplorerController.Explore(new ProfileController(Model.Owner));
            sec1.Add(owner);
            var followers = new StyledStringElement ("Followers", "" + Model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => ad.ExplorerController.Explore(new RepoFollowersController(Model.Owner, Model.Slug));
            sec1.Add(followers);
            
            var sec2 = new Section() {
                new ImageStringElement("Events", () => ad.ExplorerController.Explore(new RepoEventsController(Model.Owner, Model.Slug)),
                                       UIImage.FromBundle("Images/repoevents.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
            };
            
            if (Model.HasIssues) 
                sec2.Add(new ImageStringElement("Issues", UIImage.FromBundle("Images/flag.png")) 
                         { Accessory = UITableViewCellAccessory.DisclosureIndicator });
            if (Model.HasWiki)
                sec2.Add(new ImageStringElement("Wiki", UIImage.FromBundle("Images/pencil.png"))
                         { Accessory = UITableViewCellAccessory.DisclosureIndicator });

            var sec3 = new Section() {
                new ImageStringElement("Branches", () => ad.ExplorerController.Explore(new BranchController(Model.Owner, Model.Slug)),
                                       UIImage.FromBundle("Images/branch.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                new ImageStringElement("Tags", () => ad.ExplorerController.Explore(new RepoEventsController(Model.Owner, Model.Slug)),
                                        UIImage.FromBundle("Images/tag.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
            };
            
            Root.Add(new [] { sec1, sec2, sec3 });
        }

        protected override RepositoryDetailedModel OnUpdate()
        {
            return Model;
        }
    }
}