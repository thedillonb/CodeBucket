using System;
using System.Reactive.Disposables;
using CodeBucket.Core.ViewModels.Accounts;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewCells
{
    public class AccountTableViewCell : BaseTableViewCell<AccountItemViewModel>
    {
        public static NSString Key = new NSString(typeof(AccountTableViewCell).Name);
        public new readonly UIImageView ImageView;
        public readonly UILabel TitleLabel;
        public readonly UILabel SubtitleLabel;

        public AccountTableViewCell(IntPtr handle)
            : base(handle)
        {
            ImageView = new UIImageView();
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Layer.MinificationFilter = CoreAnimation.CALayer.FilterTrilinear;
            ImageView.Layer.MasksToBounds = true;

            TitleLabel = new UILabel();
            TitleLabel.TextColor = UIColor.FromWhiteAlpha(0.0f, 1f);
            TitleLabel.Font = UIFont.FromName("HelveticaNeue", 17f);

            SubtitleLabel = new UILabel();
            SubtitleLabel.TextColor = UIColor.FromWhiteAlpha(0.1f, 1f);
            SubtitleLabel.Font = UIFont.FromName("HelveticaNeue-Thin", 14f);

            ContentView.Add(ImageView);
            ContentView.Add(TitleLabel);
            ContentView.Add(SubtitleLabel);

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Username, x => x.TitleLabel.Text)
                    .AddTo(disposable);

                this.OneWayBind(ViewModel, x => x.Domain, x => x.SubtitleLabel.Text)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.AvatarUrl)
                    .Subscribe(x => ImageView.SetAvatar(new Core.Utils.Avatar(x)))
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.Selected)
                    .Subscribe(x => Accessory = x ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None)
                    .AddTo(disposable);
            });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var imageSize = Bounds.Height - 30f;
            ImageView.Layer.CornerRadius = imageSize / 2;
            ImageView.Frame = new CGRect(15, 15, imageSize, imageSize);

            var titlePoint = new CGPoint(ImageView.Frame.Right + 15f, 19f);
            TitleLabel.Frame = new CGRect(titlePoint.X, titlePoint.Y, ContentView.Bounds.Width - titlePoint.X - 10f, TitleLabel.Font.LineHeight);
            SubtitleLabel.Frame = new CGRect(titlePoint.X, TitleLabel.Frame.Bottom, ContentView.Bounds.Width - titlePoint.X - 10f, SubtitleLabel.Font.LineHeight + 1);
        }
    }
}

