using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Linq;
using System.Text;


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
                var desc = (x.Message ?? "").Replace("\n", " ").Trim();
                var el = new NameTimeStringElement() { Name = x.Author, Time = x.Utctimestamp, String = desc, Lines = 4 };
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

            _header = new HeaderView(0f) { Title = "Commit: " + node };
            Root.Add(new Section(_header));
        }

        protected override void OnRefresh()
        {
            var sec = new Section();
            _header.Subtitle = "Commited " + DateTime.Parse(Model.Utctimestamp).ToDaysAgo();

            var d = new MultilineElement(Model.Author) { Value = Model.Message };

            /*
             * , Model.Message, UITableViewCellStyle.Subtitle) { 
                Font = UIFont.SystemFontOfSize(15f), SubtitleFont = UIFont.SystemFontOfSize(12f) 
            };
            */

            sec.Add(d);

            if (Repo != null)
            {
                var repo = new StyledElement(Repo.Name, Images.Repo)
                { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, Lines = 1, Font = UIFont.SystemFontOfSize(15f), TextColor = UIColor.Gray };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
                sec.Add(repo);
            }

            var sec2 = new Section();

            Model.Files.ForEach(x => 
            {
                var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                var sse = new SubcaptionElement(file, x.Type)
                                                 { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                                                   LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                                                   Lines = 1 };
                sse.Tapped += () => {
                    string parent = null;
                    if (Model.Parents != null && Model.Parents.Count > 0)
                        parent = Model.Parents[0];

                    var type = x.Type.Trim().ToLower();
                    NavigationController.PushViewController(new ChangesetDiffController(User, Slug, Model.Node, parent, x.File)
                                                            { Removed = type.Equals("removed"), Added = type.Equals("added") }, true);
                };
                sec2.Add(sse);
            });


            InvokeOnMainThread(delegate {
                _header.SetNeedsDisplay();
                Root.Add(new [] { sec, sec2 });
                ReloadData();
            });
        }

        protected override ChangesetModel OnUpdate()
        {
            var x = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetInfo();
            x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            return x;
        }
    }

    public class ChangesetDiffController : SourceInfoController
    {
        protected string _parent;
        public bool Removed { get; set; }
        public bool Added { get; set; }

        public ChangesetDiffController(string user, string slug, string branch, string parent, string path)
            : base(user, slug, branch, path)
        {
            _parent = parent;
        }

        protected override string RequestData()
        {

            if (Removed && _parent == null)
            {
                throw new InvalidOperationException("File does not exist!");
            }


            var newSource = "";
            if (!Removed)
            {
                newSource = System.Security.SecurityElement.Escape(
                    Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path).Data);
            }

            var oldSource = "";
            if (_parent != null && !Added)
            {
                try
                {
                    oldSource = System.Security.SecurityElement.Escape(
                    Application.Client.Users[_user].Repositories[_slug].Branches[_parent].Source.GetFile(_path).Data);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to get old source (parent: " + _parent + ") - " + e.Message);
                }
            }

            var differ = new DiffPlex.DiffBuilder.InlineDiffBuilder(new DiffPlex.Differ());
            var a = differ.BuildDiffModel(oldSource, newSource);

            var builder = new StringBuilder();
            foreach (var k in a.Lines)
            {
                if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                    builder.Append("<span style='background-color: #ffe0e0;'>" + k.Text + "</span>");
                else if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                    builder.Append("<span style='background-color: #e0ffe0;'>" + k.Text + "</span>");
                else if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Modified)
                    builder.Append("<span style='background-color: #ffffe0;'>" + k.Text + "</span>");
                else
                    builder.Append(k.Text);

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}

