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
            if (Model.Count == 0)
                return;

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
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Commit";
            Root.UnevenRows = true;
        }

        protected override void OnRefresh()
        {
            var sec = new Section();
            //var details = new Section(new HeaderView(View.Bounds.Width) { Title = Node, Subtitle = "Commited " + DateTime.Parse(Model.Utctimestamp).ToDaysAgo() });

            //Add the big info thing for the change

            var d = new MultilineElement(Model.Author) { Value = Model.Message };

            sec.Add(d);

            if (Repo != null)
            {
                var repo = new StyledElement("Repository", Repo.Name, UITableViewCellStyle.Value1)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, Lines = 1 };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
                sec.Add(repo);
            }

            var sec2 = new Section("Changes");

            Model.Files.ForEach(x => 
            {
                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                var sse = new SubcaptionElement(file, x.Type)
                                                 { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                                                   LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                                                   Lines = 1 };
                sse.Tapped += () => NavigationController.PushViewController(new SourceInfoController(User, Slug, Model.Node, x.File), true);
                sec2.Add(sse);
            });


            InvokeOnMainThread(delegate {
                var r = new RootElement(Title);
                r.UnevenRows = true;
                r.Add(new [] { sec , sec2 });
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

