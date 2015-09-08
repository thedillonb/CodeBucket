using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.Views.Repositories
{
    public class ReadmeView : WebView
    {
        public new ReadmeViewModel ViewModel
        {
            get { return (ReadmeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public ReadmeView() : base(false)
        {
            Title = "Readme";
            Web.ScalesPageToFit = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel.Bind(x => x.Path, x => LoadFile(x));
            NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem(UIKit.UIBarButtonSystemItem.Action, (s, e) => ShareButtonPress());
            ViewModel.LoadCommand.Execute(false);
        }

        protected override bool ShouldStartLoad(Foundation.NSUrlRequest request, UIKit.UIWebViewNavigationType navigationType)
        {
            if (!request.Url.AbsoluteString.StartsWith("file://", System.StringComparison.Ordinal))
            {
                ViewModel.GoToLinkCommand.Execute(request.Url.AbsoluteString);
                return false;
            }

            return base.ShouldStartLoad(request, navigationType);
        }

        private void ShareButtonPress()
        {
            var sheet = MonoTouch.Utilities.GetSheet();
            var shareButton = sheet.AddButton("Share");
            var showButton = sheet.AddButton("Show in Bitbucket");
            var cancelButton = sheet.AddButton("Cancel");
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {
                BeginInvokeOnMainThread(() =>
                {
                if (e.ButtonIndex == showButton)
                    ViewModel.GoToGitHubCommand.Execute(null);
                else if (e.ButtonIndex == shareButton)
                    ViewModel.ShareCommand.Execute(null);
                });
            };

            sheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
        }
    }
}

