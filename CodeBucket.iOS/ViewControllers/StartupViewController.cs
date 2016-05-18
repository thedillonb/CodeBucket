using System;
using UIKit;
using CodeBucket.Core.ViewModels.App;
using Foundation;
using SDWebImage;
using CodeBucket.Core.Utils;
using CodeBucket.ViewControllers.Accounts;
using CodeBucket.ViewControllers.Application;
using MonoTouch.SlideoutNavigation;
using ReactiveUI;

namespace CodeBucket.ViewControllers
{
    public class StartupViewController : BaseViewController
    {
        const float imageSize = 128f;
        private UIImageView _imgView;
        private UILabel _statusLabel;
        private UIActivityIndicatorView _activityView;

        public StartupViewModel ViewModel { get; } = new StartupViewModel();

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            _imgView.Frame = new CoreGraphics.CGRect(View.Bounds.Width / 2 - imageSize / 2, View.Bounds.Height / 2 - imageSize / 2 - 30f, imageSize, imageSize);
            _statusLabel.Frame = new CoreGraphics.CGRect(0, _imgView.Frame.Bottom + 10f, View.Bounds.Width, 18f);
            _activityView.Center = new CoreGraphics.CGPoint(View.Bounds.Width / 2, _statusLabel.Frame.Bottom + 16f + 16F);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;

            _imgView = new UIImageView();
            _imgView.Layer.CornerRadius = imageSize / 2;
            _imgView.Layer.MasksToBounds = true;
            _imgView.Image = Images.Avatar;
            _imgView.Layer.BorderWidth = 2f;
            _imgView.Layer.BorderColor = UIColor.White.CGColor;
            Add(_imgView);

            _statusLabel = new UILabel();
            _statusLabel.TextAlignment = UITextAlignment.Center;
            _statusLabel.Font = UIFont.FromName("HelveticaNeue", 13f);
            _statusLabel.TextColor = UIColor.FromWhiteAlpha(0.9f, 1.0f);
            Add(_statusLabel);

            _activityView = new UIActivityIndicatorView { HidesWhenStopped = true };
            _activityView.Color = UIColor.FromWhiteAlpha(0.85f, 1.0f);
            Add(_activityView);

            View.BackgroundColor = Theme.CurrentTheme.PrimaryColor;

            OnActivation(d =>
            {
                ViewModel.WhenAnyValue(x => x.Avatar).Subscribe(UpdatedImage).AddTo(d);
                ViewModel.WhenAnyValue(x => x.Status).Subscribe(x => _statusLabel.Text = x).AddTo(d);
                ViewModel.GoToMenuCommand.Subscribe(GoToMenu).AddTo(d);
                ViewModel.GoToAccountsCommand.Subscribe(GoToAccounts).AddTo(d);
                ViewModel.GoToLoginCommand.Subscribe(GoToNewAccount).AddTo(d);
                ViewModel.WhenAnyValue(x => x.IsLoggingIn).Subscribe(x =>
                {
                    if (x)
                        _activityView.StartAnimating();
                    else
                        _activityView.StopAnimating();

                    _activityView.Hidden = !x;
                }).AddTo(d);
            });
        }

        private void GoToMenu(object o)
        {
            var vc = new MenuViewController();
            var slideoutController = new SlideoutNavigationController();
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            slideoutController.MenuViewController = new MenuNavigationController(vc, slideoutController);
            //vc.ViewModel.GoToDefaultTopView.Execute(null);
            var vm = new Core.ViewModels.Commits.CommitsViewModel("thedillonb", "bitbucketbrowser", "master");
            vc.NavigationController.PushViewController(new Commits.CommitsViewController() { ViewModel = vm }, true);
            slideoutController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(slideoutController, true, null);
        }

        private void GoToNewAccount(object o)
        {
            var vc = new LoginViewController();
            var nav = new ThemedNavigationController(vc);
            PresentViewController(nav, true, null);
        }

        private void GoToAccounts(object o)
        {
            var vc = new AccountsViewController();
            var nav = new ThemedNavigationController(vc);
            PresentViewController(nav, true, null);
        }

        public void UpdatedImage(Avatar avatar)
        {
            var avatarUrl = avatar?.ToUrl(Convert.ToInt32(imageSize));
            if (avatarUrl == null)
            {
                AssignUnknownUserImage();
            }
            else
            {
                _imgView.SetImage(new NSUrl(avatarUrl), Images.Avatar, 0, (img, err, cache, type) => {
                    _imgView.Image = Images.Avatar;
                    UIView.Transition(_imgView, 0.50f, UIViewAnimationOptions.TransitionCrossDissolve, () => _imgView.Image = img, null);
                });
            }
        }

        private void AssignUnknownUserImage()
        {
            _imgView.Image = Images.Avatar;
        }

        public override void ViewWillAppear(bool animated)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Fade);
            AssignUnknownUserImage();
            _statusLabel.Text = "";

            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.Fade);
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			ViewModel.StartupCommand.Execute(null);
		}

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _imgView.Image = null;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.Default;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
            return UIInterfaceOrientationMask.All;
        }

        public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, Action completionHandler)
        {
            if (PresentedViewController != null)
            {
                PresentedViewController.PresentViewController(viewControllerToPresent, animated, completionHandler);
            }
            else
            {
                base.PresentViewController(viewControllerToPresent, animated, completionHandler);
            }
        }
    }
}

