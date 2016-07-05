using System;
using UIKit;
using Foundation;
using WebKit;
using CodeBucket.Utilities;

namespace CodeBucket.ViewControllers
{
    public class BasicWebViewController : BaseViewController
    {
        private readonly LoadingIndicator _loadingIndicator = new LoadingIndicator();
        protected UIBarButtonItem BackButton;
        protected UIBarButtonItem RefreshButton;
        protected UIBarButtonItem ForwardButton;

        public WKWebView Web { get; private set; }
        private readonly bool _navigationToolbar;
        private readonly  bool _showPageAsTitle;

        public BasicWebViewController()
            : this(true, true)
        {
        }

        public BasicWebViewController(bool navigationToolbar, bool showPageAsTitle = false)
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "" };

            _navigationToolbar = navigationToolbar;
            _showPageAsTitle = showPageAsTitle;

            if (_navigationToolbar)
            {
                var backButton = BackButton = new UIBarButtonItem { Image = Images.Web.Back, Enabled = false };
                var forwardButton = ForwardButton = new UIBarButtonItem { Image = Images.Web.Forward, Enabled = false };
                var refreshButton = RefreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh) { Enabled = false };

                BackButton.TintColor = Theme.CurrentTheme.WebButtonTint;
                ForwardButton.TintColor = Theme.CurrentTheme.WebButtonTint;
                RefreshButton.TintColor = Theme.CurrentTheme.WebButtonTint;

                OnActivation(d =>
                {
                    backButton.GetClickedObservable().Subscribe(_ => Web.GoBack()).AddTo(d);
                    forwardButton.GetClickedObservable().Subscribe(_ => Web.GoForward()).AddTo(d);
                    refreshButton.GetClickedObservable().Subscribe(_ => Web.Reload()).AddTo(d);
                });

                ToolbarItems = new []
                { 
                    BackButton,
                    new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 40f },
                    ForwardButton,
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    RefreshButton
                };
            }

            EdgesForExtendedLayout = UIRectEdge.None;
        }

        private class NavigationDelegate : WKNavigationDelegate
        {
            private readonly WeakReference<BasicWebViewController> _webView;

            public NavigationDelegate(BasicWebViewController webView)
            {
                _webView = new WeakReference<BasicWebViewController>(webView);
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                _webView.Get()?.OnLoadFinished(webView, navigation);
            }

            public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
            {
                _webView.Get()?.OnLoadStarted(null, EventArgs.Empty);
            }

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                _webView.Get()?.OnLoadError(error);
            }

            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                var ret = _webView.Get()?.ShouldStartLoad(webView, navigationAction) ?? true;
                decisionHandler(ret ? WKNavigationActionPolicy.Allow : WKNavigationActionPolicy.Cancel);
            }
        }

        protected virtual bool ShouldStartLoad (WKWebView webView, WKNavigationAction navigationAction)
        {
            return true;
        }

        protected virtual void OnLoadError (NSError error)
        {
            _loadingIndicator.Down();

            if (BackButton != null)
            {
                BackButton.Enabled = Web.CanGoBack;
                ForwardButton.Enabled = Web.CanGoForward;
                RefreshButton.Enabled = true;
            }
        }

        protected virtual void OnLoadStarted (object sender, EventArgs e)
        {
            _loadingIndicator.Up();

            if (RefreshButton != null)
                RefreshButton.Enabled = false;
        }

        protected virtual void OnLoadFinished(WKWebView webView, WKNavigation navigation)
        {
            _loadingIndicator.Down();

            if (BackButton != null)
            {
                BackButton.Enabled = Web.CanGoBack;
                ForwardButton.Enabled = Web.CanGoForward;
                RefreshButton.Enabled = true;
            }

            if (_showPageAsTitle)
            {
                Web.EvaluateJavaScript("document.title", (o, _) => {
                    Title = o as NSString;
                });
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Web = new WKWebView(View.Bounds, new WKWebViewConfiguration());
            Web.NavigationDelegate = new NavigationDelegate(this);
            Add(Web);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            Web.Frame = View.Bounds;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var bounds = View.Bounds;
            if (_navigationToolbar)
                bounds.Height -= NavigationController.Toolbar.Frame.Height;
            Web.Frame = bounds;

            if (_navigationToolbar)
            {
                BackButton.Enabled = Web.CanGoBack;
                ForwardButton.Enabled = Web.CanGoForward;
                RefreshButton.Enabled = !Web.IsLoading;
            }   

            if (_showPageAsTitle)
            {
                Web.EvaluateJavaScript("document.title", (o, _) => {
                    Title = o as NSString;
                });
            }

            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            Web.Frame = View.Bounds;
        }
    }
}

