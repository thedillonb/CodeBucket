using System;
using CodeBucket.Services;
using CodeBucket.Core.ViewModels;
using UIKit;
using WebKit;
using Foundation;

namespace CodeBucket.ViewControllers
{
    public class WebBrowserViewController : WebViewController<WebBrowserViewModel>
    {
        protected UIBarButtonItem BackButton;
        protected UIBarButtonItem RefreshButton;
        protected UIBarButtonItem ForwardButton;

        public WebBrowserViewController()
        {
            BackButton = new UIBarButtonItem{ Image = Images.Web.Back, Enabled = false };
            ForwardButton = new UIBarButtonItem { Image = Images.Web.Forward, Enabled = false };
            RefreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh) { Enabled = false };

            BackButton.TintColor = Theme.CurrentTheme.WebButtonTint;
            ForwardButton.TintColor = Theme.CurrentTheme.WebButtonTint;
            RefreshButton.TintColor = Theme.CurrentTheme.WebButtonTint;

            OnActivation(disposable =>
            {
                BackButton
                    .GetClickedObservable()
                    .Subscribe(_ => Web.GoBack())
                    .AddTo(disposable);

                ForwardButton
                    .GetClickedObservable()
                    .Subscribe(_ => Web.GoForward())
                    .AddTo(disposable);

                RefreshButton
                    .GetClickedObservable()
                    .Subscribe(_ => Web.Reload())
                    .AddTo(disposable);
            });

            ToolbarItems = new[]
            {
                BackButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 40f },
                ForwardButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                RefreshButton
            };
        }

        public WebBrowserViewController(string url)
            : this()
        {
            ViewModel = new WebBrowserViewModel(url);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                if (!string.IsNullOrEmpty(ViewModel.Url))
                    Web.LoadRequest(new NSUrlRequest(new NSUrl(ViewModel.Url)));
            }
            catch (Exception e)
            {
                AlertDialogService.ShowAlert("Unable to process request!", e.Message);
            }
        }

        public override void OnLoadError(NSError error)
        {
            base.OnLoadError(error);
            BackButton.Enabled = Web.CanGoBack;
            ForwardButton.Enabled = Web.CanGoForward;
            RefreshButton.Enabled = true;
        }

        public override void OnLoadStarted(object sender, EventArgs e)
        {
            base.OnLoadStarted(sender, e);
            RefreshButton.Enabled = false;
        }

        public override void OnLoadFinished(WKWebView webView, WKNavigation navigation)
        {
            base.OnLoadFinished(webView, navigation);
            BackButton.Enabled = Web.CanGoBack;
            ForwardButton.Enabled = Web.CanGoForward;
            RefreshButton.Enabled = true;
            GetTitle();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        protected static string JavaScriptStringEncode(string data)
        {
            return System.Web.HttpUtility.JavaScriptStringEncode(data);
        }

        protected static string UrlDecode(string data)
        {
            return System.Web.HttpUtility.UrlDecode(data);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //var bounds = View.Bounds;
            //if (_navigationToolbar)
            //    bounds.Height -= NavigationController.Toolbar.Frame.Height;
            //Web.Frame = bounds;


            BackButton.Enabled = Web.CanGoBack;
            ForwardButton.Enabled = Web.CanGoForward;
            RefreshButton.Enabled = !Web.IsLoading;

            GetTitle();

            NavigationController.SetToolbarHidden(false, animated);
        }

        private void GetTitle()
        {
            Web.EvaluateJavaScript("document.title", (o, _) =>
            {
                ViewModel.PageTitle = o as NSString;
            });
        }
    }
}

