using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;


namespace BitbucketBrowser.UI
{
    public class ExploreController : DialogViewController
    {
        public ExploreController()
            : base(UITableViewStyle.Plain, null, false)
        {
            EnableSearch = true;
            AutoHideSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Root = new RootElement(Title);
        }

        public override void SearchButtonClicked(string text)
        {

        }


        public void Explore(UIViewController view)
        {
            var ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
            var nav = (UINavigationController)ad.TabController.SelectedViewController;
            NavigationController.PopToRootViewController(false);
            ad.TabController.SelectedViewController = NavigationController;
            NavigationController.PushViewController(view, true);
        }
    }
}

