using System;
using Foundation;
using UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using SDWebImage;

namespace CodeBucket.Cells
{
    public partial class CommitCellView : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("CommitCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("CommitCellView");
        private static nfloat DefaultContentConstraintSize = 0.0f;

        public CommitCellView(IntPtr handle) 
            : base(handle)
        {
        }

        public static CommitCellView Create() 
            => Nib.Instantiate(null, null).GetValue(0) as CommitCellView;

        public nint MaxContentLines
        {
            get { return ContentLabel.Lines; }
            set { ContentLabel.Lines = value; }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            MainImageView.Layer.MasksToBounds = true;
            MainImageView.Layer.CornerRadius = MainImageView.Frame.Height / 2f;
            SeparatorInset = new UIEdgeInsets(0, TitleLabel.Frame.Left, 0, 0);

            TitleLabel.TextColor = Theme.CurrentTheme.MainTitleColor;
            TimeLabel.TextColor = UIColor.Gray;
            ContentLabel.TextColor = Theme.CurrentTheme.MainTextColor;
            DefaultContentConstraintSize = ContentConstraint.Constant;
        }

        public void Bind(string name, string description, string time, UIImage logoImage, string logoUri)
        {
            TitleLabel.Text = name;
            TimeLabel.Text = time;
            ContentLabel.Text = description;
            ContentLabel.Hidden = string.IsNullOrWhiteSpace(description);
            ContentConstraint.Constant = ContentLabel.Hidden ? 0 : DefaultContentConstraintSize;

            if (logoUri == null)
                MainImageView.Image = logoImage;
            else
                MainImageView.SetImage(new NSUrl(logoUri), logoImage);
        }
    }
}

