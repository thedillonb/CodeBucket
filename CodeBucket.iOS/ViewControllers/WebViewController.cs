using System;
using UIKit;
using Foundation;
using WebKit;
using CodeBucket.Utilities;

namespace CodeBucket.ViewControllers
{
    public interface IWebViewControllerEvents
    {
        bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction);

        void OnLoadError(NSError error);

        void OnLoadStarted(object sender, EventArgs e);

        void OnLoadFinished(WKWebView webView, WKNavigation navigation);
    }

    public class WebNavigationDelegate : WKNavigationDelegate
    {
        private readonly WeakReference<IWebViewControllerEvents> _webView;

        public WebNavigationDelegate(IWebViewControllerEvents webView)
        {
            _webView = new WeakReference<IWebViewControllerEvents>(webView);
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

    public abstract class WebViewController<TViewModel> : BaseViewController<TViewModel>, IWebViewControllerEvents
        where TViewModel : class
    {
        private readonly LoadingIndicator _loadingIndicator = new LoadingIndicator();
        private IDisposable _loadingDisposable;

        public WKWebView Web { get; private set; }

        protected WebViewController()
        {
            EdgesForExtendedLayout = UIRectEdge.None;
        }

        public virtual bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            return true;
        }

        public virtual void OnLoadError (NSError error)
        {
        }

        public virtual void OnLoadStarted (object sender, EventArgs e)
        {
        }

        public virtual void OnLoadFinished(WKWebView webView, WKNavigation navigation)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Web = new WKWebView(View.Bounds, new WKWebViewConfiguration());
            Web.NavigationDelegate = new WebNavigationDelegate(this);
            Web.AutoresizingMask = UIViewAutoresizing.All;
            Add(Web);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _loadingDisposable = Web.AddObserver("estimatedProgress", NSKeyValueObservingOptions.New, ProgressObserver);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _loadingDisposable?.Dispose();
            _loadingDisposable = null;
        }

        public void ProgressObserver(NSObservedChange nsObservedChange)
        {
            var progress = Convert.ToInt32(Web.EstimatedProgress);
            if (progress != 1 && _loadingIndicator.Value == 0)
                _loadingIndicator.Up();
            else if (progress == 1)
                _loadingIndicator.Down();
        }

        protected void LoadContent(string content)
        {
            Web.LoadHtmlString(content ?? string.Empty, NSBundle.MainBundle.BundleUrl);
        }

        protected string LoadFile(string path)
        {
            if (path == null) return string.Empty;
            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(uri))));
            return uri;
        }
    }
}

