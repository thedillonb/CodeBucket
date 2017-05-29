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
                    .SelectUnit()
                    .BindCommand(this, x => x.ViewModel.ShowMenuCommand)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.ContentText)
                    .Subscribe(x => ContentText = x)
                    .AddTo(disposable);
            });
        }

        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            var url = navigationAction.Request.Url.AbsoluteString;
            if (!url.StartsWith("file://", StringComparison.Ordinal))
            {
                var webBrowser = new WebBrowserViewController(url);
                PresentViewController(webBrowser, true, null);
                return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }
    }
}

