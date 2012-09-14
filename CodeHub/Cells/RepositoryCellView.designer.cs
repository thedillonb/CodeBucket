// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CodeHub
{
	[Register ("RepositoryCellView")]
	partial class RepositoryCellView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel Description { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Image3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Label3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Caption { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Label1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel Label2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Image1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView Image2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel RepoName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Description != null) {
				Description.Dispose ();
				Description = null;
			}

			if (Image3 != null) {
				Image3.Dispose ();
				Image3 = null;
			}

			if (Label3 != null) {
				Label3.Dispose ();
				Label3 = null;
			}

			if (Caption != null) {
				Caption.Dispose ();
				Caption = null;
			}

			if (Label1 != null) {
				Label1.Dispose ();
				Label1 = null;
			}

			if (Label2 != null) {
				Label2.Dispose ();
				Label2 = null;
			}

			if (Image1 != null) {
				Image1.Dispose ();
				Image1 = null;
			}

			if (Image2 != null) {
				Image2.Dispose ();
				Image2 = null;
			}

			if (RepoName != null) {
				RepoName.Dispose ();
				RepoName = null;
			}
		}
	}
}
