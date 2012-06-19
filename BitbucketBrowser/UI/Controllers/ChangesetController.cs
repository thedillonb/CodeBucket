using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;


namespace BitbucketBrowser.UI
{
    public class ChangesetController : Controller<ChangesetsModel>
    {
        public string User { get; private set; }

        public string Slug { get; private set; }

        public ChangesetController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Changes";
            Root.Add(new Section());
            Root.UnevenRows = true;
        }


        protected override void OnRefresh ()
        {
            var items = new List<Element>();
            Model.Changesets.ForEach(x => {
                var el = new MessageElement() { Sender = x.Author, Body = (x.Message ?? "").Replace("\n", " ").Trim(), Date = DateTime.Parse(x.Timestamp), Subject = "" };
                items.Add(el);
            });

            InvokeOnMainThread(delegate {
                Root[0].Clear();
                Root[0].AddAll(items);
            });
        }

        protected override ChangesetsModel OnUpdate ()
        {
            var client = new BitbucketSharp.Client("thedillonb", "djames");
            return client.Users[User].Repositories[Slug].Changesets.GetChangesets();
        }

    }

    public class ChangesetInfoController : Controller<ChangesetModel>
    {
        public string Node { get; private set; }

        public string User { get; private set; }

        public string Slug { get; private set; }

        public ChangesetInfoController(string user, string slug, string node)
            : base(true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Commit";
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

            var auth = new StyledStringElement("Author", Model.Author, UITableViewCellStyle.Value1)
            { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
            auth.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Author), true);

            var desc = new StyledMultilineElement(DateTime.Parse(Model.Utctimestamp).ToDaysAgo(), Model.Message, MonoTouch.UIKit.UITableViewCellStyle.Subtitle)
            { Font = UIFont.SystemFontOfSize(12f), SubtitleFont = UIFont.SystemFontOfSize(14f), DetailColor = UIColor.Black, TextColor = UIColor.LightGray };

            RootElement root = new RootElement(Title) { 
                new Section("Details") { auth, desc },
                new Section("Changes") { Elements = sec },
            };

            root.UnevenRows = true;

            BeginInvokeOnMainThread(delegate {
                Root = root;
            });
        }

        protected override ChangesetModel OnUpdate()
        {
            var client = new BitbucketSharp.Client("thedillonb", "djames");
            return client.Users[User].Repositories[Slug].Changesets[Node].GetInfo();
        }
    }
}

