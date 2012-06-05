using System.Drawing;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;

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

            Root.Add(new Section(new HeaderView(View.Bounds.Width) { 
                Title = Model.Name, Subtitle = lastUpdated
            }));
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
            
            var owner = new StyledStringElement("Owner", Model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledStringElement ("Followers", "" + Model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(Model.Owner, Model.Slug), true);
            sec1.Add(followers);
            
            var sec2 = new Section() {
                new ImageStringElement("Events", UIImage.FromBundle("Images/repoevents.png"))
            };
            
            if (Model.HasIssues) 
                sec2.Add(new ImageStringElement("Issues", UIImage.FromBundle("Images/flag.png")));
            if (Model.HasWiki)
                sec2.Add(new ImageStringElement("Wiki", UIImage.FromBundle("Images/pencil.png")));
            
            
            Root.Add(new [] { sec1, sec2 });
        }

        protected override RepositoryDetailedModel OnUpdate()
        {
            return Model;
        }
    }
}