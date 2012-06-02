using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

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
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			User.ShouldReturn = delegate {
				Password.BecomeFirstResponder();
				return true;
			};
			Password.ShouldReturn = delegate {
				Password.ResignFirstResponder();
				BeginLogin();
				return true;
			};
			LoginButton.TouchUpInside += delegate {
				BeginLogin();
			};
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			
			this.NavigationController.SetNavigationBarHidden(true, false);
		}
		
		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			
			this.NavigationController.SetNavigationBarHidden(false, true);
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
			Console.WriteLine("Cool!");
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

