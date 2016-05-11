using System;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views;
using UIKit;
using WebKit;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Repositories
{
    public class ReadmeViewController : WebViewController<ReadmeViewModel>
    {
        private readonly UIBarButtonItem _actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);

        public ReadmeViewController() : base(false)
        {
            Title = "Readme";
            NavigationItem.RightBarButtonItem = _actionButton;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.WhenAnyValue(x => x.ViewModel.ContentText)
                .IsNotNull()
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(LoadContent);

            ViewModel.LoadCommand.Execute(null);
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
            if (!navigationAction.Request.Url.AbsoluteString.StartsWith("file://", StringComparison.Ordinal))
            {
                var webBrowser = new WebBrowserViewController();
                NavigationController.PushViewController(webBrowser, true);
                return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }

        private void ShareButtonPress(object o, EventArgs args)
        {
            if (ViewModel.ShowMenuCommand.CanExecute(_actionButton))
                ViewModel.ShowMenuCommand.Execute(_actionButton);
        }
    }
}

