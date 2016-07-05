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
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;

            OnActivation(disposable =>
            {
                actionButton
                    .GetClickedObservable()
                    .InvokeCommand(ViewModel.ShowMenuCommand)
                    .AddTo(disposable);
            });

            this.WhenAnyValue(x => x.ViewModel.ContentText)
                .IsNotNull()
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(LoadContent);
        }

        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            var url = navigationAction.Request.Url.AbsoluteString;
            if (!url.StartsWith("file://", StringComparison.Ordinal))
            {
                var webBrowser = new WebBrowserViewController(url);
                NavigationController.PushViewController(webBrowser, true);
                return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }
    }
}

