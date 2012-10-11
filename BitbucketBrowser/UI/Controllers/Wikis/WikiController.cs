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
using BitbucketBrowser.Controllers;
using MonoTouch;


namespace BitbucketBrowser.UI.Controllers.Wikis
{
    public class WikiInfoController : WebViewController
    {
        private static string WikiCache = Utilities.BaseDir + "/Documents/WikiCache/";
        private string _user, _slug, _page;
        private ErrorView _errorView;
        private bool _isVisible = false;

        private void Load(string page, bool push = true, bool forceInvalidation = false)
        {
            this.DoWork(() => {
                if (_errorView != null)
                {
                    InvokeOnMainThread(delegate {
                        _errorView.RemoveFromSuperview();
                        _errorView = null;
                    });
                }

                var url = RequestAndSave(page, forceInvalidation);
                var escapedUrl = Uri.EscapeUriString("file://" + url);

                InvokeOnMainThread(delegate {
                    Web.ScalesPageToFit = false;
                    Web.LoadRequest(NSUrlRequest.FromUrl(new NSUrl(escapedUrl)));
                });
            },
            (ex) => {
                if (_isVisible)
                    Utilities.ShowAlert("Unable to Find Wiki Page", ex.Message);
            });
        }

        public WikiInfoController(string user, string slug, string page = "Home")
            : base()
        {
            _user = user;
            _slug = slug;
            _page = page;
            Title = "Wiki";
            Web.DataDetectorTypes = UIDataDetectorType.None;
            Web.ShouldStartLoad = ShouldStartLoad;
        }

        public override void ViewDidDisappear(bool animated)
        {
            _isVisible = false;
            base.ViewDidDisappear(animated);
            if (System.IO.Directory.Exists(WikiCache))
                System.IO.Directory.Delete(WikiCache, true);
        }

        public override void ViewDidAppear(bool animated)
        {
            _isVisible = true;
            base.ViewDidAppear(animated);

            //Delete the cache directory just incase it already exists..
            if (System.IO.Directory.Exists(WikiCache))
                System.IO.Directory.Delete(WikiCache, true);
            System.IO.Directory.CreateDirectory(WikiCache);

            //Load the page
            Load(_page);
        }

        private bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navType)
        {
            if (request.Url.ToString().Substring(0, 7).Equals("file://"))
                Web.ScalesPageToFit = false;
            else
                Web.ScalesPageToFit = true;

            if (navType == UIWebViewNavigationType.LinkClicked) 
            {
                if (request.Url.ToString().Substring(0, 7).Equals("wiki://"))
                {
                    Load(request.Url.ToString().Substring(7));
                    return false;
                }
            }

            return true;
        }

        protected override void OnLoadFinished(object sender, EventArgs e)
        {
            base.OnLoadFinished(sender, e);
            Title = Web.EvaluateJavascript("document.title");
        }

        private string RequestAndSave(string page, bool forceInvalidation)
        {
            var wiki = Application.Client.Users[_user].Repositories[_slug].Wikis[page];
            var d = wiki.GetInfo(forceInvalidation);
            var w = new Wiki.CreoleParser();
            w.OnLink += HandleOnLink;

            //Generate the markup
            var markup = new System.Text.StringBuilder();
            markup.Append("<html><head><title>");
            markup.Append(page);
            markup.Append("</title></head><body>");
            markup.Append(w.ToHTML(d.Data));
            markup.Append("</body></html>");

            var url = WikiCache + page + ".html";
            using (var file = System.IO.File.Create(url))
            {
                using (var writer = new System.IO.StreamWriter(file))
                {
                    writer.Write(markup.ToString());
                }
            }

            return url;
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

