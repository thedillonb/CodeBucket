using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using RedPlum;

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
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void SearchButtonClicked(string text)
        {
            this.View.EndEditing(true);


            MBProgressHUD hud = null;
            hud = new MBProgressHUD(this.View.Superview); 
            hud.Mode = MBProgressHUDMode.Indeterminate;
            hud.TitleText = "Searching...";

            InvokeOnMainThread(delegate {
                Root.Clear();
                this.View.Superview.AddSubview(hud);
                hud.Show(true);
            });

            ThreadPool.QueueUserWorkItem(delegate {
                var l = Application.Client.Repositories.Search(text);
                var elements = new List<Element>(l.Repositories.Count);
                l.Repositories.ForEach(r => 
                {
                    var el = new StyledStringElement(r.Name, r.Description, UITableViewCellStyle.Subtitle)
                    { Accessory = UITableViewCellAccessory.DisclosureIndicator, Lines = 1, LineBreakMode = UILineBreakMode.TailTruncation };
                    el.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(r), true);
                    elements.Add(el);
                });


                InvokeOnMainThread(delegate {
                    Root.Clear();
                    Root.Add(new Section() { Elements = elements });
                    if (hud != null)
                    {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    }
                });
            });
        }
    }
}

