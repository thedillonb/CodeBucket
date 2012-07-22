using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Text;
using System.Net;

namespace BitbucketBrowser
{
	public partial class LoginViewController : UIViewController
	{
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

            Title = "Add Account";
            View.BackgroundColor = UIColor.Clear;
			
			User.ShouldReturn = delegate {
				Password.BecomeFirstResponder();
				return true;
			};
			Password.ShouldReturn = delegate {
				Password.ResignFirstResponder();
				BeginLogin();
				return true;
			};
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}
		
		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
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
            var client = new BitbucketSharp.Client(User.Text, Password.Text);

            try
            {
                var a = client.Account.SSHKeys.GetKeys();
            }
            catch (Exception ex)
            {
                //This means its a bad username & password
                var a = new UIAlertView();
                a.Title = "Unable to Authenticate";
                a.Message = "Unable to login as user " + User.Text + ". Please check your credentials and try again.";
                a.DismissWithClickedButtonIndex(a.AddButton("Ok"), true);
                a.Show();
                return;
            }

            //Logged in correctly!
            //Go back to the other view and add the username
            Application.Accounts.Add(new Account() { Username = User.Text, Password = Password.Text });
            NavigationController.PopViewControllerAnimated(true);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

