using System;
using UIKit;
using Foundation;
using CodeBucket.Core.ViewModels;
using WebKit;
using CodeBucket.Utilities;
using CodeBucket.ViewControllers;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Views
{
    public class WebViewController<TViewModel> : WebViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        private readonly LoadingIndicator _loadIndicator = new LoadingIndicator();

        private TViewModel _viewModel;
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        public WebViewController(bool navigationToolbar = true, bool showPageAsTitle = true)
            : base(navigationToolbar, showPageAsTitle)
        {
            this.WhenAnyValue(x => x.ViewModel)
                .OfType<ILoadableViewModel>()
                .Select(x => x.LoadCommand.IsExecuting)
                .Switch()
                .Subscribe(x =>
                {
                    if (x) _loadIndicator.Up();
                    else _loadIndicator.Down();
                });
        }
    }

    public class WebViewController : BaseViewController
    {
        protected UIBarButtonItem BackButton;
        protected UIBarButtonItem RefreshButton;
        protected UIBarButtonItem ForwardButton;

        public WKWebView Web { get; private set; }
        private readonly bool _navigationToolbar;
        private readonly  bool _showPageAsTitle;

        protected virtual void GoBack()
        {
            Web.GoBack();
        }

        protected virtual void Refresh()
        {
            Web.Reload();
        }

        protected virtual void GoForward()
        {
            Web.GoForward();
        }

        public WebViewController(bool navigationToolbar = true, bool showPageAsTitle = true)
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem() { Title = "" };

            _navigationToolbar = navigationToolbar;
            _showPageAsTitle = showPageAsTitle;

            if (_navigationToolbar)
            {
                BackButton = new UIBarButtonItem(Images.Web.Back, UIBarButtonItemStyle.Plain, (s, e) => GoBack()) { Enabled = false };
                ForwardButton = new UIBarButtonItem(Images.Web.Forward, UIBarButtonItemStyle.Plain, (s, e) => GoForward()) { Enabled = false };
                RefreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (s, e) => Refresh()) { Enabled = false };

                BackButton.TintColor = Theme.CurrentTheme.WebButtonTint;
                ForwardButton.TintColor = Theme.CurrentTheme.WebButtonTint;
                RefreshButton.TintColor = Theme.CurrentTheme.WebButtonTint;
            }

            EdgesForExtendedLayout = UIRectEdge.None;
        }

        private class NavigationDelegate : WKNavigationDelegate
        {
            private readonly WeakReference<WebViewController> _webView;

            public NavigationDelegate(WebViewController webView)
            {
                _webView = new WeakReference<WebViewController>(webView);
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
            NetworkActivity.PopNetworkActive();

            if (BackButton != null)
            {
                BackButton.Enabled = Web.CanGoBack;
                ForwardButton.Enabled = Web.CanGoForward;
                RefreshButton.Enabled = true;
            }
        }

        protected virtual void OnLoadStarted (object sender, EventArgs e)
        {
            NetworkActivity.PushNetworkActive();

            if (RefreshButton != null)
                RefreshButton.Enabled = false;
        }

        protected virtual void OnLoadFinished(WKWebView webView, WKNavigation navigation)
        {
            NetworkActivity.PopNetworkActive();

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

        protected static string JavaScriptStringEncode(string data)
        {
            return System.Web.HttpUtility.JavaScriptStringEncode(data);
        }

        protected static string UrlDecode(string data)
        {
            return System.Web.HttpUtility.UrlDecode(data);
        }

        protected string LoadFile(string path)
        {
            if (path == null)
                return string.Empty;

            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            InvokeOnMainThread(() => Web.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(uri))));
            return uri;
        }

        protected void LoadContent(string content)
        {
            Web.LoadHtmlString(content, NSBundle.MainBundle.BundleUrl);
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
                ToolbarItems = new []
                { 
                    BackButton,
                    new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 40f },
                    ForwardButton,
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    RefreshButton
                };

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

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (_navigationToolbar)
                ToolbarItems = null;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            Web.Frame = View.Bounds;
        }
    }
}

