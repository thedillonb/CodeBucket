using UIKit;
using System;

namespace CodeBucket.ViewControllers.Walkthrough
{
    public partial class FeedbackViewController : BaseViewController
    {
        public FeedbackViewController()
            : base("FeedbackViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            GitHubButton.BackgroundColor = UIColor.FromRGB(0x7f, 0x8c, 0x8d);
            GitHubButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            GitHubButton.Layer.CornerRadius = 6f;

            UserVoiceButton.BackgroundColor = UIColor.FromRGB(0x2c, 0x3e, 0x50);
            UserVoiceButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            UserVoiceButton.Layer.CornerRadius = 6f;

            OnActivation(d => {
                GitHubButton
                    .GetClickedObservable()
                    .Subscribe(_ => ShowWebPage("https://github.com/thedillonb/codehub"))
                    .AddTo(d);
                UserVoiceButton
                    .GetClickedObservable()
                    .Subscribe(_ => ShowWebPage("https://codehub.uservoice.com"))
                    .AddTo(d);
            });
        }

        private void ShowWebPage(string url)
        {
            var view = new WebBrowserViewController(url);
            view.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.Buttons.Cancel, UIBarButtonItemStyle.Done, 
                (s, e) => DismissViewController(true, null));
            PresentViewController(new ThemedNavigationController(view), true, null);
        }
    }
}

