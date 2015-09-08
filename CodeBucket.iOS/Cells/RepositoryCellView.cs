using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SDWebImage;

namespace CodeBucket.Cells
{
    public partial class RepositoryCellView : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("RepositoryCellView", NSBundle.MainBundle);
        public static NSString Key = new NSString("RepositoryCellView");
        private static nfloat DefaultContentConstraint = 0f;

        public RepositoryCellView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            RepositoryName.TextColor = Theme.CurrentTheme.MainTitleColor;
            RepositoryDescription.TextColor = Theme.CurrentTheme.MainTextColor;
            RepositoryImage.Layer.MasksToBounds = true;
            RepositoryImage.Layer.CornerRadius = RepositoryImage.Bounds.Height / 2f;
            DefaultContentConstraint = ContentConstraint.Constant;
        }

        public void Bind(string name, string description, string repoOwner, UIImage logoImage, Uri logoUri)
        {
            RepositoryName.Text = name;
            RepositoryOwner.Text = repoOwner;
            RepositoryDescription.Hidden = string.IsNullOrWhiteSpace(description);
            RepositoryDescription.Text = description ?? string.Empty;
            ContentConstraint.Constant = RepositoryDescription.Hidden ? 0 : DefaultContentConstraint;

            if (logoUri == null)
                RepositoryImage.Image = logoImage;
            else
                RepositoryImage.SetImage(new NSUrl(logoUri.AbsoluteUri), logoImage);
        }
    }
}

