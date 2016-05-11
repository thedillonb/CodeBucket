using System;
using Foundation;
using UIKit;
using SDWebImage;
using ObjCRuntime;
using CodeBucket.Core.Utils;

namespace CodeBucket.TableViewCells
{
    public partial class RepositoryCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("RepositoryCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("RepositoryCellView");
        private static nfloat DefaultContentConstraint = 0f;

        public static RepositoryCellView Create()
        {
            var views = NSBundle.MainBundle.LoadNib(Key, null, null);
            return Runtime.GetNSObject<RepositoryCellView>(views.ValueAt(0));
        }

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

        public void Bind(string name, string description, string repoOwner, Avatar logoUri)
        {
            RepositoryName.Text = name;
            RepositoryOwner.Text = repoOwner;
            RepositoryDescription.Hidden = string.IsNullOrWhiteSpace(description);
            RepositoryDescription.Text = description ?? string.Empty;
            ContentConstraint.Constant = RepositoryDescription.Hidden ? 0 : DefaultContentConstraint;

            var uri = logoUri.ToUri(64);

            if (uri == null)
                RepositoryImage.Image = Images.RepoPlaceholder;
            else
                RepositoryImage.SetImage(new NSUrl(uri.AbsoluteUri), Images.RepoPlaceholder);
        }
    }
}

