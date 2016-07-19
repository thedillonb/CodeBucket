using System;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Views;
using UIKit;
using WebKit;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Repositories
{
    public sealed class ReadmeViewController : WebViewController<ReadmeViewModel>
    {
        private string _contentText;
        private string ContentText
        {
            get { return _contentText; }
            set { this.RaiseAndSetIfChanged(ref _contentText, value); }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;

            this.WhenAnyValue(x => x.ContentText)
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(LoadContent);

            OnActivation(disposable =>
            {
                actionButton
                    .GetClickedObservable()
                    .InvokeCommand(this, x => x.ViewModel.ShowMenuCommand)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.ContentText)
                    .Subscribe(x => ContentText = x)
                    .AddTo(disposable);
            });
        }

        protected override void Navigate(UIViewController viewController)
        {
            if (viewController is WebBrowserViewController)
                this.PresentModal(viewController);
            else
                base.Navigate(viewController);
        }

        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            var url = navigationAction.Request.Url.AbsoluteString;
            if (!url.StartsWith("file://", StringComparison.Ordinal))
            {
                var webBrowser = new WebBrowserViewController(url);
                this.PresentModal(webBrowser);
                return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }
    }
}

