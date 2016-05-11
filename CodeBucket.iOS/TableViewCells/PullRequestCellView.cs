using System;
using Foundation;
using UIKit;
using SDWebImage;
using CodeBucket.Core.Utils;
using ObjCRuntime;

namespace CodeBucket.TableViewCells
{
    public partial class PullRequestCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("PullRequestCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PullRequestCellView");

        public PullRequestCellView()
        {
        }

        public PullRequestCellView(IntPtr handle) 
            : base(handle)
        {
        }

        public static PullRequestCellView Create()
        {
            var views = NSBundle.MainBundle.LoadNib("PullRequestCellView", null, null);
            return Runtime.GetNSObject<PullRequestCellView>(views.ValueAt(0));
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

        public void Bind(string title, string time, Avatar avatar)
        {
            TitleLabel.Text = title;
            TimeLabel.Text = time;
            var avatarUrl = avatar?.ToUrl(64);

            if (avatarUrl == null)
                MainImageView.Image = Images.Avatar;
            else
                MainImageView.SetImage(new NSUrl(avatarUrl), Images.Avatar);
        }
    }
}

