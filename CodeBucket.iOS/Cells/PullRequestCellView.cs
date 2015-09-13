using System;
using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using SDWebImage;
using CodeBucket.Core.Utils;

namespace CodeBucket.Cells
{
    public partial class PullRequestCellView : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("PullRequestCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PullRequestCellView");

        public PullRequestCellView(IntPtr handle) 
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            MainImageView.Layer.MasksToBounds = true;
            MainImageView.Layer.CornerRadius = MainImageView.Frame.Height / 2f;
            ContentView.Opaque = true;

            SeparatorInset = new UIEdgeInsets(0, TitleLabel.Frame.Left, 0, 0);
            TitleLabel.TextColor = Theme.CurrentTheme.MainTitleColor;
            TimeLabel.TextColor = Theme.CurrentTheme.MainSubtitleColor;
        }

        public void Bind(string title, string time, Avatar avatar, UIImage placeholderImage)
        {
            TitleLabel.Text = title;
            TimeLabel.Text = time;
            var avatarUrl = avatar?.ToUrl();

            if (avatarUrl == null)
                MainImageView.Image = placeholderImage;
            else
                MainImageView.SetImage(new NSUrl(avatarUrl), placeholderImage);
        }
    }
}

