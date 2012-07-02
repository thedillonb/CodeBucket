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
            Root.Add(new Section());
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
                Root[0].Insert(0, UITableViewRowAnimation.Top, items);
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

        private HeaderView _header;

        public ChangesetInfoController(string user, string slug, string node)
            : base(true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Commit";

            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { Title = "Change: " + node };
            Root.Add(new Section(_header));

        }

        protected override void OnRefresh()
        {
            var sec = new List<Element>(Model.Files.Count);
            Model.Files.ForEach(x => 
            {
                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                var sse = new StyledStringElement(file, x.Type, MonoTouch.UIKit.UITableViewCellStyle.Subtitle)
                                                 { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                                                   LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                                                   Lines = 1 };
                sse.Tapped += () => NavigationController.PushViewController(new SourceInfoController(User, Slug, Model.Node, x.File), true);
                sec.Add(sse);
            });

            var details = new Section();

            if (!string.IsNullOrEmpty(Model.Message))
            {
                var desc = new MultilineElement(Model.Message);
                details.Insert(0, desc);
            }

            var author = new StyledStringElement("Author", Model.Author, UITableViewCellStyle.Value1)
            { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
            author.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Author), true);
            details.Add(author);

            if (Repo != null)
            {
                var repo = new StyledStringElement("Repository", Repo.Name, UITableViewCellStyle.Value1)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
                details.Add(repo);
            }

            _header.Subtitle = "Commited " + DateTime.Parse(Model.Utctimestamp).ToDaysAgo();

            var changes = new Section("Changes") { Elements = sec };

            BeginInvokeOnMainThread(delegate {
                _header.SetNeedsDisplay();
                Root.Insert(1, UITableViewRowAnimation.Fade, new [] { details, changes });
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

