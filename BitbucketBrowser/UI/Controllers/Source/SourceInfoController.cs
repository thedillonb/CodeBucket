using System;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using MonoTouch.Foundation;
using CodeFramework.UI.Views;

namespace BitbucketBrowser.UI.Controllers.Source
{
    
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
            var d = Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path, true);
            return System.Security.SecurityElement.Escape(d.Data);
        }
        
        private void Request()
        {
            this.DoWork(() => {
                var data = RequestData();
                
                InvokeOnMainThread(delegate {
                    var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
                    var filled = html.Replace("{DATA}", data);
                    
                    var url = NSBundle.MainBundle.BundlePath + "/SourceBrowser";
                    url = url.Replace("/", "//").Replace(" ", "%20");
                    
                    _web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));
                });
            }, (ex) => {
                ErrorView.Show(this.View, ex.Message);
            });
        }
    }
}

