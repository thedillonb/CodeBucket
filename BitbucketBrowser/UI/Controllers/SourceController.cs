using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Threading;
using RedPlum;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using CodeFramework.UI.Views;


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
            AutoHideSearch = true;
            EnableSearch = true;

            if (string.IsNullOrEmpty(path))
                Title = "Source";
            else
            {
                Title = path.Substring(path.LastIndexOf('/') + 1);
            }
        }


        protected override void OnRefresh ()
        {
            var sec = new Section();
            Model.Directories.ForEach(d => 
            {
                sec.Add(new StyledElement(d, 
                                          () => NavigationController.PushViewController(new SourceController(Username, Slug, Branch, Path + "/" + d), true),
                                          Images.Folder));
            });

            Model.Files.ForEach(f =>
            {
                var i = f.Path.LastIndexOf('/') + 1;
                var p = f.Path.Substring(i);
                sec.Add(new StyledElement(p,() => NavigationController.PushViewController(
                                          new SourceInfoController(Username, Slug, Branch, f.Path) { Title = p }, true), 
                                          Images.File));
            });

            if (sec.Count == 0)
                return;

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override SourceModel OnUpdate ()
        {
            return Application.Client.Users[Username].Repositories[Slug].Branches[Branch].Source[Path].GetInfo();
        }

    }


    public class SourceInfoController : UIViewController
    {
        private UIWebView _web;

        protected string _user, _slug, _branch, _path;


        public SourceInfoController(string user, string slug, string branch, string path)
            : base()
        {
            _user = user;
            _slug = slug;
            _branch = branch;
            _path = path;

            _web = new UIWebView();
            _web.DataDetectorTypes = UIDataDetectorType.None;
            _web.ScalesPageToFit = true;
            this.Add(_web);

            Title = path.Substring(path.LastIndexOf('/') + 1);
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

        protected virtual string RequestData()
        {
            var d = Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path);
            return System.Security.SecurityElement.Escape(d.Data);
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
                    var data = RequestData();

                    InvokeOnMainThread(delegate {
                        var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
                        var filled = html.Replace("{DATA}", data);

                        var url = NSBundle.MainBundle.BundlePath + "/SourceBrowser";
                        url = url.Replace("/", "//").Replace(" ", "%20");

                        //var url = NSBundle.PathForResourceAbsolute("SourceBrowser", 
                        _web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));

                        //_web.LoadHtmlString("<pre>" + data + "</pre>", null);
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

