using System;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views;
using UIKit;
using WebKit;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Repositories
{
    public class ReadmeViewController : WebView
    {
        private readonly UIBarButtonItem _actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);

        public new ReadmeViewModel ViewModel
        {
            get { return (ReadmeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public ReadmeViewController() : base(false)
        {
            Title = "Readme";
            NavigationItem.RightBarButtonItem = _actionButton;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.Bind(x => x.ContentText, true)
                .IsNotNull()
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(LoadContent);

            ViewModel.LoadCommand.Execute(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _actionButton.Clicked += ShareButtonPress;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _actionButton.Clicked -= ShareButtonPress;
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

        private void ShareButtonPress(object o, EventArgs args)
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

            sheet.ShowFrom(_actionButton, true);
        }
    }
}

