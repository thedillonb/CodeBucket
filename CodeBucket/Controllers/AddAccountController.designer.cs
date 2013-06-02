// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using MonoTouch.Foundation;

namespace CodeBucket.Controllers
{
	[Register ("AddAccountController")]
	partial class AddAccountController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton BitbucketButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton GitHubButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Logo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BitbucketButton != null) {
				BitbucketButton.Dispose ();
				BitbucketButton = null;
			}

			if (GitHubButton != null) {
				GitHubButton.Dispose ();
				GitHubButton = null;
			}

			if (Logo != null) {
				Logo.Dispose ();
				Logo = null;
			}
		}
	}
}
