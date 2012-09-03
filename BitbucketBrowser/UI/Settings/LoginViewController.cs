using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Text;
using System.Net;
using RedPlum;
using System.Threading;
using BitbucketBrowser.UI;
using CodeFramework.UI.Views;

namespace BitbucketBrowser
{
	public partial class LoginViewController : UIViewController
	{

        public Action LoginComplete;

		public LoginViewController() : base ("LoginViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


            View.BackgroundColor = UIColor.FromPatternImage(Images.LogoBehind);

            Logo.Image = Images.Logo;
            Title = "Add Account";
			
			User.ShouldReturn = delegate {
				Password.BecomeFirstResponder();
				return true;
			};
			Password.ShouldReturn = delegate {
				Password.ResignFirstResponder();

                //Run this in another thread
                ThreadPool.QueueUserWorkItem(delegate { BeginLogin(); });
				return true;
			};
		}
		
		public override void ViewDidUnload()
		{
			base.ViewDidUnload();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets();
		}
		
		private void BeginLogin()
        {
            MBProgressHUD hud;
            bool successful = false;

            //The nice hud
            InvokeOnMainThread(delegate {
                hud = new MBProgressHUD(this.View); 
                hud.Mode = MBProgressHUDMode.Indeterminate;
                hud.TitleText = "Logging In...";
                this.View.AddSubview(hud);
                hud.Show(true);
            });

            try
            {
                var client = new BitbucketSharp.Client(User.Text, Password.Text);
                client.Account.SSHKeys.GetKeys();
                successful = true;
            }
            catch (Exception)
            {
            }


            InvokeOnMainThread(delegate {
                //Dismiss the hud
                hud.Hide(true);
                hud.RemoveFromSuperview();

                if (!successful)
                {
                    Alert.Show("Unable to Authenticate", "Unable to login as user " + User.Text + ". Please check your credentials and try again.");
                    return;
                }

                var newAccount = new Account() { Username = User.Text, Password = Password.Text };

                if (Application.Accounts.Exists(newAccount))
                {
                    Alert.Show("Unable to Add User", "That user already exists!");
                    return;
                }

                //Logged in correctly!
                //Go back to the other view and add the username
                Application.Accounts.Insert(newAccount);

                if (NavigationController != null)
                    NavigationController.PopViewControllerAnimated(true);

                if (LoginComplete != null)
                    LoginComplete();
            });
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

