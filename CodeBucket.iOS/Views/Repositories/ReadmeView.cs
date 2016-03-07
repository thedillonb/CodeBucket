using System;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views;
using UIKit;
using WebKit;

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
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel.Bind(x => x.Path).Subscribe(x => LoadFile(x));
            ViewModel.LoadCommand.Execute(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShareButtonPress());
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NavigationItem.RightBarButtonItem = null;
        }

        protected override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            if (!navigationAction.Request.Url.AbsoluteString.StartsWith("file://", System.StringComparison.Ordinal))
            {
                ViewModel.GoToLinkCommand.Execute(navigationAction.Request.Url.AbsoluteString);
                return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }

        private void ShareButtonPress()
        {
            var sheet = new UIActionSheet();
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

                sheet.Dispose();
            };

            sheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
        }
    }
}

