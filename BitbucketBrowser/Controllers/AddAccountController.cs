using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;
using BitbucketBrowser.Data;

namespace BitbucketBrowser.Controllers.Accounts
{
	public partial class AddAccountController : UIViewController
    {
		public Action<Account> AccountAdded;

		protected virtual void OnAccountAdded(Account account)
		{
			var e = AccountAdded;
			if (e != null)
				e(account);
		}

		public AddAccountController()
			: base("AddAccountController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
            View.BackgroundColor = UIColor.FromPatternImage(Images.LogoBehind);
            Logo.Image = Images.Logo;
            Title = "Account Type";

			BitbucketButton.TouchUpInside += BitbucketButtonTouch;
			GitHubButton.TouchUpInside += GitHubButtonTouch;
        }

        void GitHubButtonTouch (object sender, EventArgs e)
        {

        }

        void BitbucketButtonTouch (object sender, EventArgs e)
        {
			var login = new Bitbucket.Controllers.Accounts.LoginViewController();
			login.LoginComplete = AccountAdded;
			NavigationController.PushViewController(login, true);
        }

        [Obsolete("Deprecated in iOS 6.0")]
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            ReleaseDesignerOutlets();
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                    return true;
            }
            else
            {
                // Return true for supported orientations
                return true;
            }

            return false;
        }
    }
}

