using System;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;


namespace BitbucketBrowser.UI
{
    public class WikiController : Controller<WikiModel>
    {
        public string User { get; private set; }

        public string Repo { get; private set; }

        public string Page { get; private set; }

        public WikiController(string user, string repo, string page = "Home")
            : base(true, true)
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Tags";
            User = user;
            Repo = repo;
            Page = page;
            Root.Add(new Section());
        }

        protected override void OnRefresh ()
        {
        }

        protected override WikiModel OnUpdate ()
        {
            return Application.Client.Users[User].Repositories[Repo].Wikis[Page].GetInfo();
        }

    }

    
    public class WikiInfoController : UIViewController
    {
        private UIWebView _web;

        private string _user, _slug, _page;


        public WikiInfoController(string user, string slug, string page = "Home")
            : base()
        {
            _user = user;
            _slug = slug;
            _page = page;

            _web = new UIWebView();
            _web.DataDetectorTypes = UIDataDetectorType.None;

            this.Add(_web);

            Title = page;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _web.Frame = this.View.Bounds;
            Request();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            var bounds = View.Bounds;
            _web.Frame = bounds;
        }

        private void Request()
        {
            var hud = new MBProgressHUD(this.View.Superview); 
            hud.Mode = MBProgressHUDMode.Indeterminate;
            hud.TitleText = "Loading...";
            this.View.Superview.AddSubview(hud);
            hud.Show(true);


            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    var d = Application.Client.Users[_user].Repositories[_slug].Wikis[_page].GetInfo();


                    var w = new Wiki.CreoleParser();
                    var markup = w.ToHTML(d.Data);

                    InvokeOnMainThread(delegate {
                        _web.LoadHtmlString(markup, null);
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    });
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                        ErrorView.Show(this.View, e.Message);
                    });
                }
            });

        }


    }
}

