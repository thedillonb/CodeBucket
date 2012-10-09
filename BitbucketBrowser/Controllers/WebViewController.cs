using System;
using MonoTouch.UIKit;
using CodeFramework.UI.Views;

namespace BitbucketBrowser.Controllers
{
    public class WebViewController : UIViewController
    {
        protected UIBarButtonItem _back, _refresh, _forward;
        public UIWebView Web { get; private set; }

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
            : base()
        {
            Title = "Wiki";
            Web = new UIWebView();
            Web.LoadFinished += OnLoadFinished;
            Web.LoadStarted += OnLoadStarted;
            
            ToolbarItems = new [] { 
                (_back = new UIBarButtonItem(Images.BackNavigationButton, UIBarButtonItemStyle.Plain, (s, e) => { GoBack(); }) { Enabled = false }),
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 40f },
                (_forward = new UIBarButtonItem(Images.ForwardNavigationButton, UIBarButtonItemStyle.Plain, (s, e) => { GoForward(); }) { Enabled = false }),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                (_refresh = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (s, e) =>  { Refresh(); })),
            };
        }

        protected virtual void OnLoadStarted (object sender, EventArgs e)
        {
            MonoTouch.Utilities.PushNetworkActive();
        }

        protected virtual void OnLoadFinished (object sender, EventArgs e)
        {
            MonoTouch.Utilities.PopNetworkActive();
            _back.Enabled = Web.CanGoBack;
            _forward.Enabled = Web.CanGoForward;
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, animated);
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            WatermarkView.AssureWatermark(this);
            this.Add(Web);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
            var bounds = View.Bounds;
            bounds.Height -= NavigationController.Toolbar.Frame.Height;
            Web.Frame = bounds;
        }
        
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            var bounds = View.Bounds;
            Web.Frame = bounds;
        }
    }
}

