// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace BitbucketBrowser
{
	[Register ("RepositoryInfoCellView")]
	partial class RepositoryInfoCellView
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView Image1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Image2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Image1 != null) {
				Image1.Dispose ();
				Image1 = null;
			}

			if (Image2 != null) {
				Image2.Dispose ();
				Image2 = null;
			}
		}
	}
}
