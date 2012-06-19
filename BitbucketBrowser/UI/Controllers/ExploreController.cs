using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace BitbucketBrowser.UI
{
    public class ExploreController : DialogViewController
    {
        public ExploreController()
            : base(UITableViewStyle.Plain, new RootElement("Explore"), false)
        {
            EnableSearch = true;
            AutoHideSearch = false;
            Root.Add(new Section());
        }

        public override void SearchButtonClicked(string text)
        {
            ThreadPool.QueueUserWorkItem(delegate {
                var client = new BitbucketSharp.Client("thedillonb", "djames");
                var l = client.Repositories.Search(text);
                var elements = new List<Element>(l.Repositories.Count);
                l.Repositories.ForEach(r => 
                {
                    var el = new StyledStringElement(r.Name, r.Description, UITableViewCellStyle.Subtitle)
                    { Accessory = UITableViewCellAccessory.DisclosureIndicator, Lines = 1, LineBreakMode = UILineBreakMode.TailTruncation };
                    el.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(r), true);
                    elements.Add(el);
                });


                InvokeOnMainThread(delegate {
                    ;                    Root.Clear();
                    Root.Add(new Section() { Elements = elements });
                });
            });
        }
    }
}

