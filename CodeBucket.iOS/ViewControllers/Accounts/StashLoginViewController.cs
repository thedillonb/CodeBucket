using CoreGraphics;
using Foundation;
using UIKit;
using CodeBucket.Core.Services;
using System;
using ReactiveUI;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Utilities;
using Splat;

namespace CodeBucket.ViewControllers.Accounts
{
    public partial class StashLoginViewController : BaseViewController
    {
        private readonly UIColor EnterpriseBackgroundColor = UIColor.FromRGB(0x20, 0x50, 0x81);

        public StashLoginViewModel ViewModel { get; } = new StashLoginViewModel();

        public StashLoginViewController()
            : base("StashLoginViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Login";

            View.BackgroundColor = EnterpriseBackgroundColor;
            Logo.Image = Images.StashLogo.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            Logo.TintColor = UIColor.FromRGB(239, 239, 244);

            LoginButton.BackgroundColor = UIColor.FromRGB(0x10, 0x40, 0x71);
            LoginButton.Enabled = true;

            LoginButton.Layer.BorderColor = UIColor.Black.CGColor;
            LoginButton.Layer.BorderWidth = 1;
            LoginButton.Layer.CornerRadius = 4;

            var attributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.LightGray
            };

            Domain.AttributedPlaceholder = new NSAttributedString("Domain", attributes);
            User.AttributedPlaceholder = new NSAttributedString("Username", attributes);
            Password.AttributedPlaceholder = new NSAttributedString("Password", attributes);

            foreach (var i in new[] { Domain, User, Password })
            {
                i.BackgroundColor = UIColor.FromRGB(0x10, 0x40, 0x71);
                i.Layer.BorderColor = UIColor.Black.CGColor;
                i.Layer.BorderWidth = 1;
                i.Layer.CornerRadius = 4;
            }

            Domain.ShouldReturn = delegate
            {
                User.BecomeFirstResponder();
                return true;
            };

            User.ShouldReturn = delegate
            {
                Password.BecomeFirstResponder();
                return true;
            };
            Password.ShouldReturn = delegate
            {
                Password.ResignFirstResponder();
                LoginButton.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                return true;
            };

            OnActivation(d =>
            {
                User.GetChangedObservable().Subscribe(x => ViewModel.Username = x).AddTo(d);
                Password.GetChangedObservable().Subscribe(x => ViewModel.Password = x).AddTo(d);
                Domain.GetChangedObservable().Subscribe(x => ViewModel.Domain = x).AddTo(d);
                LoginButton.GetClickedObservable().BindCommand(ViewModel.LoginCommand).AddTo(d);
                ViewModel.WhenAnyValue(x => x.IsLoggingIn).SubscribeStatus("Logging in...").AddTo(d);
                ViewModel.LoginCommand.ThrownExceptions.Subscribe(HandleLoginException).AddTo(d);
            });
        }

        private void HandleLoginException(Exception e)
        {
            var alert = Locator.Current.GetService<IAlertDialogService>();
            alert.Alert("Unable to Login!", "Unable to login user " + ViewModel.Username + ": " + e.Message);
        }

        NSObject _hideNotification, _showNotification;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHideNotification);
            _showNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_hideNotification);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_showNotification);
        }

        private void OnKeyboardHideNotification(NSNotification notification)
        {
            ScrollView.ContentInset = UIEdgeInsets.Zero;
            ScrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var inset = new UIEdgeInsets(0, 0, keyboardFrame.Height, 0);
            ScrollView.ContentInset = inset;
            ScrollView.ScrollIndicatorInsets = inset;
        }
    }
}
