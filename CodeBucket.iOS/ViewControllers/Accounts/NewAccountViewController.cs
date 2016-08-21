using System;
using CoreGraphics;
using UIKit;
using CodeBucket.Core.Services;
using CodeBucket.ViewControllers.Application;
using Splat;

namespace CodeBucket.ViewControllers.Accounts
{
    public class NewAccountViewController : BaseViewController
    {
        private readonly UIColor DotComBackgroundColor = UIColor.FromRGB(239, 239, 244);
        private readonly UIColor EnterpriseBackgroundColor = UIColor.FromRGB(0x20, 0x50, 0x81);

        public NewAccountViewController()
        {
            Title = "New Account";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var dotComButton = new AccountButton("Bitbucket", Images.BitbucketLogo);
            dotComButton.BackgroundColor = DotComBackgroundColor;
            dotComButton.Label.TextColor = EnterpriseBackgroundColor;
            dotComButton.ImageView.TintColor = EnterpriseBackgroundColor;

            var enterpriseButton = new AccountButton("Stash", Images.StashLogo.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate));
            enterpriseButton.BackgroundColor = EnterpriseBackgroundColor;
            enterpriseButton.Label.TextColor = dotComButton.BackgroundColor;
            enterpriseButton.ImageView.TintColor = dotComButton.BackgroundColor;

            Add(dotComButton);
            Add(enterpriseButton);

            View.ConstrainLayout(() =>
                dotComButton.Frame.Top == View.Frame.Top &&
                dotComButton.Frame.Left == View.Frame.Left &&
                dotComButton.Frame.Right == View.Frame.Right &&
                dotComButton.Frame.Bottom == View.Frame.GetMidY() &&

                enterpriseButton.Frame.Top == dotComButton.Frame.Bottom &&
                enterpriseButton.Frame.Left == View.Frame.Left &&
                enterpriseButton.Frame.Right == View.Frame.Right &&
                enterpriseButton.Frame.Bottom == View.Frame.Bottom);

            OnActivation(disposable =>
            {
                dotComButton
                    .GetClickedObservable()
                    .Subscribe(_ => DotComButtonTouch())
                    .AddTo(disposable);
                enterpriseButton
                    .GetClickedObservable()
                    .Subscribe(_ => EnterpriseButtonTouch())
                    .AddTo(disposable);
            });
        }

        private void DotComButtonTouch()
        {
            NavigationController.PushViewController(new LoginViewController(), true);
        }

        private void EnterpriseButtonTouch()
        {
            var features = Locator.Current.GetService<IFeaturesService>();
            if (features.IsProEnabled)
            {
                NavigationController.PushViewController(new StashLoginViewController(), true);
            }
            else
            {
                this.PresentUpgradeViewController();
            }
        }

        private class AccountButton : UIButton
        {
            public UILabel Label { get; private set; }

            public new UIImageView ImageView { get; private set; }

            public AccountButton(string text, UIImage image)
                : base(new CGRect(0, 0, 100, 100))
            {
                Label = new UILabel(new CGRect(0, 0, 100, 100));
                Label.Text = text;
                Label.TextAlignment = UITextAlignment.Center;
                Label.Font = UIFont.PreferredHeadline;
                Add(Label);

                ImageView = new UIImageView(new CGRect(0, 0, 100, 100));
                ImageView.Image = image;
                ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                Add(ImageView);

                this.ConstrainLayout(() =>
                    ImageView.Frame.Top == this.Frame.Top + 30f &&
                    ImageView.Frame.Left == this.Frame.Left &&
                    ImageView.Frame.Right == this.Frame.Right &&
                    ImageView.Frame.Bottom == this.Frame.Bottom - 60f &&

                    Label.Frame.Top == ImageView.Frame.Bottom + 10f &&
                    Label.Frame.Left == this.Frame.Left &&
                    Label.Frame.Right == this.Frame.Right &&
                    Label.Frame.Height == 20);
            }
        }
    }
}