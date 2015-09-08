// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CodeBucket.Cells
{
	[Register ("RepositoryCellView")]
	partial class RepositoryCellView
	{
		[Outlet]
		UIKit.NSLayoutConstraint ContentConstraint { get; set; }

		[Outlet]
		UIKit.UILabel RepositoryDescription { get; set; }

		[Outlet]
		UIKit.UIImageView RepositoryImage { get; set; }

		[Outlet]
		UIKit.UILabel RepositoryName { get; set; }

		[Outlet]
		UIKit.UILabel RepositoryOwner { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContentConstraint != null) {
				ContentConstraint.Dispose ();
				ContentConstraint = null;
			}

			if (RepositoryDescription != null) {
				RepositoryDescription.Dispose ();
				RepositoryDescription = null;
			}

			if (RepositoryImage != null) {
				RepositoryImage.Dispose ();
				RepositoryImage = null;
			}

			if (RepositoryName != null) {
				RepositoryName.Dispose ();
				RepositoryName = null;
			}

			if (RepositoryOwner != null) {
				RepositoryOwner.Dispose ();
				RepositoryOwner = null;
			}
		}
	}
}
