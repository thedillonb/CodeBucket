using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Users;
using Foundation;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewCells
{
    public class UserTableViewCell : BaseTableViewCell<UserItemViewModel>
    {
        public static NSString Key = new NSString(typeof(UserTableViewCell).Name);

        public UserTableViewCell(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        public UserTableViewCell()
            : base(UITableViewCellStyle.Default, Key)
        {
            Initialize();
        }

        private void Initialize()
        {
            SeparatorInset = new UIEdgeInsets(0, 48f, 0, 0);
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            ImageView.Layer.CornerRadius = 16f;
            ImageView.Layer.MasksToBounds = true;

            this.WhenAnyValue(x => x.ViewModel)
                .Subscribe(x =>
                {
                    TextLabel.Text = x?.Username;
                    ImageView.SetAvatar(x?.Avatar);
                });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ImageView.Frame = new CoreGraphics.CGRect(6, 6, 32, 32);
            TextLabel.Frame = new CoreGraphics.CGRect(48, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
            if (DetailTextLabel != null)
                DetailTextLabel.Frame = new CoreGraphics.CGRect(48, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
        }
    }
}

