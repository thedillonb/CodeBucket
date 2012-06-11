using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;


namespace BitbucketBrowser.UI
{
    public class SourceController : Controller<SourceModel>
    {
        public string Username { get; private set; }

        public string Slug { get; private set; }

        public string Branch { get; private set; }

        public string Path { get; private set; }

        public SourceController(string username, string slug, string branch = "master", string path = "")
            : base(true, false)
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Username = username;
            Slug = slug;
            Branch = branch;
            Path = path;
            Root.Add(new Section());

            if (string.IsNullOrEmpty(path))
                Title = "Source";
            else
            {
                Title = path.Substring(path.LastIndexOf('/') + 1);
            }
        }


        protected override void OnRefresh ()
        {
            var items = new List<Element>(Model.Files.Count + Model.Directories.Count);
            Model.Directories.ForEach(d => 
            {
                items.Add(new ItemElement(d, () => NavigationController.PushViewController(new SourceController(Username, Slug, Branch, Path + "/" + d), true),
                                                 UIImage.FromBundle("/Images/folder.png"))
                          { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator,  });
            });

            Model.Files.ForEach(f =>
            {
                var i = f.Path.LastIndexOf('/') + 1;
                var p = f.Path.Substring(i);
                items.Add(new ItemElement(p,() => NavigationController.PushViewController(new SourceView() { Title = p}, true) ,UIImage.FromBundle("/Images/file.png")));
            });


            InvokeOnMainThread(delegate {
                Root[0].Clear();
                Root[0].AddAll(items);
            });
        }

        protected override SourceModel OnUpdate ()
        {
            var client = new BitbucketSharp.Client("thedillonb", "djames");
            return client.Users[Username].Repositories[Slug].Branches[Branch].Source[Path].GetInfo();
        }

        public class ItemElement : ImageStringElement
        {
            public ItemElement(string cap, NSAction act, UIImage img)
                : base(cap, act, img)
            {
            }

            public override UITableViewCell GetCell(UITableView tv)
            {
                var cell = base.GetCell(tv);
                cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(15f);
                return cell;
            }
        }
    }

    public class SourceView : UIViewController
    {
        UIWebView _web;

        public SourceView()
            : base()
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _web = new UIWebView(this.View.Frame);
            this.Add(_web);
        }


    }
}

