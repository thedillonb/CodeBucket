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
        private UIBarButtonItem _back, _refresh;
        private LinkedList<string> _history = new LinkedList<string>();
        private ErrorView _errorView;

        private void AddHistory(string page)
        {
            _history.AddLast(page);
            _back.Enabled = _history.Count > 1;
        }

        private void GoBack()
        {
            if (_history.Count <= 1)
                return;
            _history.RemoveLast();
            _back.Enabled = _history.Count > 1;
            Load(_history.Last.Value, false);
        }

        private void Load(string page, bool push = true, bool forceInvalidation = false)
        {
            if (push)
                AddHistory(page);

            _back.Enabled = false;
            _refresh.Enabled = false;

            this.DoWork(() => {
                if (_errorView != null)
                {
                    InvokeOnMainThread(delegate {
                        _errorView.RemoveFromSuperview();
                        _errorView = null;
                    });
                }

                var wiki = Application.Client.Users[_user].Repositories[_slug].Wikis[page];
                var d = wiki.GetInfo(forceInvalidation);
                var w = new Wiki.CreoleParser();
                w.OnLink += HandleOnLink;
                var markup = w.ToHTML(d.Data);
                
                InvokeOnMainThread(delegate {
                    _web.ScalesPageToFit = false;
                    Title = page;
                    _web.LoadHtmlString(markup, null);
                });
            }, (ex) => {
                _errorView = ErrorView.Show(this.View, ex.Message);
            }, () => {
                _back.Enabled = _history.Count > 1;
                _refresh.Enabled = true;
            });
        }


        public WikiInfoController(string user, string slug, string page = "Home")
            : base()
        {
            _user = user;
            _slug = slug;
            _page = page;
            Title = "Wiki";
            _web = new UIWebView() { DataDetectorTypes = UIDataDetectorType.None };
            _web.ShouldStartLoad = ShouldStartLoad;

            ToolbarItems = new [] { 
                (_back = new UIBarButtonItem(UIImage.FromBundle("Images/back_button"), UIBarButtonItemStyle.Plain, (s, e) => { GoBack(); }) { Enabled = false }),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                (_refresh = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (s, e) =>  { _web.Reload(); })),
            };
        }

        private bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navType)
        {
            if (navType == UIWebViewNavigationType.LinkClicked) 
            {
                var url = request.Url.ToString();
                if (url.StartsWith("wiki://"))
                {
                    var page = url.Substring(7);
                    Load(page);
                    return false;
                }
                else
                {
                    UIApplication.SharedApplication.OpenUrl(request.Url);
                    return false;
                }
            }
            else if (navType == UIWebViewNavigationType.Reload)
            {
                //Reload.
                Load(_history.Last.Value, false, true);
                return false;
            }

            return true;
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, true);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.Add(_web);
        }

        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(false, true);
            base.ViewWillAppear(animated);
            var bounds = View.Bounds;
            bounds.Height -= NavigationController.Toolbar.Frame.Height;
            _web.Frame = bounds;

            Load(_page);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            var bounds = View.Bounds;
            _web.Frame = bounds;
        }

        void HandleOnLink (object sender, Wiki.LinkEventArgs e)
        {
            if (e.Href.Contains("://"))
            {
                e.Target = Wiki.LinkEventArgs.TargetEnum.External;
            }
            else
            {
                e.Target = Wiki.LinkEventArgs.TargetEnum.Internal;
                e.Href = "wiki://" + e.Href;
            }
        }
    }
}

