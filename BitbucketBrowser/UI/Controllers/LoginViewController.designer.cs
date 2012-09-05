// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace BitbucketBrowser
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextField Password { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField User { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Logo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Password != null) {
				Password.Dispose ();
				Password = null;
			}

			if (User != null) {
				User.Dispose ();
				User = null;
			}

			if (Logo != null) {
				Logo.Dispose ();
				Logo = null;
			}
		}
	}
}
