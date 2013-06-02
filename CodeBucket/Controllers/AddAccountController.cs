using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;
using CodeBucket.Data;

namespace CodeBucket.Controllers
{
	[MonoTouch.Foundation.Register("AccountTypeButton")]
	public class AccountTypeButton : UIButton
	{
		public AccountTypeButton(IntPtr p)
			: base(p)
		{
			var gradientLayer = new MonoTouch.CoreAnimation.CAGradientLayer();
			gradientLayer.Frame = this.Layer.Bounds;
			gradientLayer.Colors = new MonoTouch.CoreGraphics.CGColor[] {
				UIColor.FromWhiteAlpha(1f, 0.95f).CGColor,
				UIColor.FromWhiteAlpha(0.85f, 0.9f).CGColor
			};
			gradientLayer.Locations = new MonoTouch.Foundation.NSNumber[] {
				0.0f,
				1.0f
			};
			gradientLayer.CornerRadius = 10;
			Layer.InsertSublayer(gradientLayer, 0);
			Layer.ShadowOffset = new System.Drawing.SizeF(2.0f, 2.0f);
			Layer.ShadowOpacity = 0.5f;
		}

		public override bool Highlighted {
			get {
				return base.Highlighted;
			}
			set {
				base.Highlighted = value;
				if (value)
				{
					Layer.ShadowOffset = new System.Drawing.SizeF(1.0f, 1.0f);
					Layer.ShadowOpacity = 0.25f;
				}
				else
				{
					Layer.ShadowOffset = new System.Drawing.SizeF(2.0f, 2.0f);
					Layer.ShadowOpacity = 0.5f;
				}
			}
		}
	}

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
            Title = "Add Account";

			BitbucketButton.SetImage(Images.BitbucketLogo, UIControlState.Normal);
			BitbucketButton.SetImage(Images.BitbucketLogo, UIControlState.Highlighted);
			BitbucketButton.ImageEdgeInsets = new UIEdgeInsets(10, 10, 10, 10);
			BitbucketButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			BitbucketButton.BringSubviewToFront(BitbucketButton.ImageView);
			BitbucketButton.TouchUpInside += BitbucketButtonTouch;

			GitHubButton.SetImage(Images.GitHubLogo, UIControlState.Normal);
			GitHubButton.SetImage(Images.GitHubLogo, UIControlState.Highlighted);
			GitHubButton.ImageEdgeInsets = new UIEdgeInsets(10, 10, 10, 10);
			GitHubButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			GitHubButton.BringSubviewToFront(GitHubButton.ImageView);
			GitHubButton.TouchUpInside += GitHubButtonTouch;
        }

        void GitHubButtonTouch (object sender, EventArgs e)
        {
			var login = new GitHub.Controllers.Accounts.GitHubLoginController();
			login.LoginComplete = AccountAdded;
			NavigationController.PushViewController(login, true);
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

