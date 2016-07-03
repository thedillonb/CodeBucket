using System;
using Foundation;
using UIKit;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.TableViewCells
{
    public partial class CommitCellView : BaseTableViewCell<CommitItemViewModel>
    {
        public static readonly UINib Nib = UINib.FromName("CommitCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("CommitCellView");
        private static nfloat DefaultContentConstraintSize = 0.0f;

        public static CommitCellView Create()
        {
            return Nib.Instantiate(null, null).GetValue(0) as CommitCellView;
        }

        public CommitCellView(IntPtr handle) 
            : base(handle)
        {
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

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(x =>
            {
                TitleLabel.Text = x.Name;
                TimeLabel.Text = x.Date;
                ContentLabel.Text = x.Description;
                ContentLabel.Hidden = string.IsNullOrWhiteSpace(x.Description);
                ContentConstraint.Constant = ContentLabel.Hidden ? 0 : DefaultContentConstraintSize;
                MainImageView.SetAvatar(x.Avatar);
            });
        }
    }
}

