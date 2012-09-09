using System;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch.Foundation;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Views;


namespace BitbucketBrowser.UI.Controllers.Wikis
{
    public class WikiInfoController : UIViewController
    {
        private UIWebView _web;
        private string _user, _slug, _page;

        public WikiInfoController(string user, string slug, string page = "Home")
            : base()
        {
            _user = user;
            _slug = slug;
            _page = Title = page;
            _web = new UIWebView() { DataDetectorTypes = UIDataDetectorType.None };
            _web.ShouldStartLoad = (webView, request, navType) => {
                if (navType == UIWebViewNavigationType.LinkClicked) 
                {
                    UIApplication.SharedApplication.OpenUrl(request.Url);
                    return false;
                }
                return true;
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.Add(_web);
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
            this.DoWork(() => {
                var d = Application.Client.Users[_user].Repositories[_slug].Wikis[_page].GetInfo();
                var w = new Wiki.CreoleParser();
                var markup = w.ToHTML(d.Data);
                
                InvokeOnMainThread(delegate {
                    _web.LoadHtmlString(markup, null);
                });
            }, (ex) => {
                ErrorView.Show(this.View, ex.Message);
            });
        }
    }
}

