using System;
using MonoTouch.UIKit;
using CodeFramework.UI.Views;

namespace BitbucketBrowser.Controllers
{
    public class WebViewController : UIViewController
    {
        protected UIBarButtonItem _back, _refresh, _forward;
        public UIWebView Web { get; private set; }
        private bool _navigationToolbar;

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
         
        public WebViewController()
            : this(true)
        {
        }

        public WebViewController(bool navigationToolbar)
            : base()
        {
            Web = new UIWebView();
            Web.ScalesPageToFit = true;
            Web.LoadFinished += OnLoadFinished;
            Web.LoadStarted += OnLoadStarted;
            Web.LoadError += OnLoadError;

            _navigationToolbar = navigationToolbar;

            if (_navigationToolbar)
            {
                ToolbarItems = new [] { 
                    (_back = new UIBarButtonItem(Images.BackNavigationButton, UIBarButtonItemStyle.Plain, (s, e) => { GoBack(); }) { Enabled = false }),
                    new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 40f },
                    (_forward = new UIBarButtonItem(Images.ForwardNavigationButton, UIBarButtonItemStyle.Plain, (s, e) => { GoForward(); }) { Enabled = false }),
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                    (_refresh = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (s, e) =>  { Refresh(); })),
                };
            }
        }

        protected virtual void OnLoadError (object sender, UIWebErrorArgs e)
        {
            MonoTouch.Utilities.PopNetworkActive();
            if (_refresh != null)
                _refresh.Enabled = true;
        }

        protected virtual void OnLoadStarted (object sender, EventArgs e)
        {
            MonoTouch.Utilities.PushNetworkActive();
            if (_refresh != null)
                _refresh.Enabled = false;
        }

        protected virtual void OnLoadFinished(object sender, EventArgs e)
        {
            MonoTouch.Utilities.PopNetworkActive();
            if (_back != null)
            {
                _back.Enabled = Web.CanGoBack;
                _forward.Enabled = Web.CanGoForward;
                _refresh.Enabled = true;
            }
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (_navigationToolbar)
                NavigationController.SetToolbarHidden(true, animated);
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            WatermarkView.AssureWatermark(this);
            this.Add(Web);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            Web.Frame = View.Bounds;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            if (_navigationToolbar)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
            var bounds = View.Bounds;
            if (_navigationToolbar)
                bounds.Height -= NavigationController.Toolbar.Frame.Height;
            Web.Frame = bounds;
        }
        
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            Web.Frame = View.Bounds;
        }
    }
}

