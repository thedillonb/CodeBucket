using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Linq;


namespace BitbucketBrowser.UI
{
    public class ChangesetController : Controller<List<ChangesetModel>>
    {
        public string User { get; private set; }

        public string Slug { get; private set; }

        private DateTime _lastUpdate = DateTime.MinValue;

        public ChangesetController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Changes";
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //TableView.SetContentOffset(new System.Drawing.PointF(0, -8), false);
            //Root.Add();
        }


        protected override void OnRefresh ()
        {
            var items = new List<Element>();
            Model.ForEach(x => {
                var el = new ChangeElement(x);
                el.Tapped += () => NavigationController.PushViewController(new ChangesetInfoController(User, Slug, x.Node), true);
                items.Add(el);
            });

            InvokeOnMainThread(delegate {
                var r = new RootElement(Title) { UnevenRows = true };
                r.Add(new Section() { Elements = items });
                Root = r;
            });
        }

        protected override List<ChangesetModel> OnUpdate ()
        {
            var changes = Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets();

            var newChanges =
                         (from s in changes.Changesets
                          where DateTime.Parse(s.Utctimestamp) > _lastUpdate
                          orderby DateTime.Parse(s.Utctimestamp) descending
                          select s).ToList();
            if (newChanges.Count > 0)
                 _lastUpdate = (from r in newChanges select DateTime.Parse(r.Utctimestamp)).Max();
            return newChanges;
        }

    }

    public class ChangesetInfoController : Controller<ChangesetModel>
    {
        public string Node { get; private set; }

        public string User { get; private set; }

        public string Slug { get; private set; }

        public RepositoryDetailedModel Repo { get; set; }

        public ChangesetInfoController(string user, string slug, string node)
            : base(true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Commit";
        }

        protected override void OnRefresh()
        {
            var sec = new List<Element>(Model.Files.Count);
            Model.Files.ForEach(x => 
            {
                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                var sse = new SubcaptionElement(file, x.Type)
                                                 { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                                                   LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                                                   Lines = 1 };
                sse.Tapped += () => NavigationController.PushViewController(new SourceInfoController(User, Slug, Model.Node, x.File), true);
                sec.Add(sse);
            });

            var details = new Section();

            var firstEl2 = new StyledElement(Node, DateTime.Parse(Model.Utctimestamp).ToString("MM/dd/yy")) { 
                BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/button")),
                Font = UIFont.BoldSystemFontOfSize(14f),
                SubtitleFont = UIFont.SystemFontOfSize(12f),
                TextColor = UIColor.White,
                DetailColor = UIColor.White 
            };

            details.Add(firstEl2);

            if (!string.IsNullOrEmpty(Model.Message))
            {
                var desc = new MultilineElement(Model.Message);
                details.Add(desc);
            }

            var author = new StyledElement("Author", Model.Author, UITableViewCellStyle.Value1)
            { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
            author.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Author), true);
            details.Add(author);

            if (Repo != null)
            {
                var repo = new StyledElement("Repository", Repo.Name, UITableViewCellStyle.Value1)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, Lines = 1 };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
                details.Add(repo);
            }

            var firstEl = new StyledElement("Changes") { 
                BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/button")),
                Font = UIFont.BoldSystemFontOfSize(14f), 
                TextColor = UIColor.White 
            };

            sec.Insert(0, firstEl);

            var changes = new Section() { Elements = sec };

            BeginInvokeOnMainThread(delegate {
                var r = new RootElement(Title) { new [] { details, changes } };
                Root = r;
            });
        }

        protected override ChangesetModel OnUpdate()
        {
            var x = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetInfo();
            x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            return x;
        }
    }
}

