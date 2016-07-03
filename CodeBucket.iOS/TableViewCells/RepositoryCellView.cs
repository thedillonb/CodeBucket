using System;
using Foundation;
using UIKit;
using CodeBucket.Core.ViewModels.Repositories;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.TableViewCells
{
    public partial class RepositoryCellView : BaseTableViewCell<RepositoryItemViewModel>
    {
        public static readonly UINib Nib = UINib.FromName("RepositoryCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("RepositoryCellView");
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

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    RepositoryName.Text = x.Name;
                    RepositoryOwner.Text = x.Owner;
                    RepositoryDescription.Hidden = string.IsNullOrWhiteSpace(x.Description);
                    RepositoryDescription.Text = x.Description ?? string.Empty;
                    ContentConstraint.Constant = RepositoryDescription.Hidden ? 0 : DefaultContentConstraint;
                    RepositoryImage.SetAvatar(x.Avatar);
                });
        }
    }
}

