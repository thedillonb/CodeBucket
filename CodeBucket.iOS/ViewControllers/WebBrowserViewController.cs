using System;
using CodeBucket.Services;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.ViewControllers
{
    public class WebBrowserViewController : WebViewController<WebBrowserViewModel>
    {
        public WebBrowserViewController()
            : base(true, true)
        {
            Title = "Web";
        }

        public WebBrowserViewController(string url)
            : this()
        {
            ViewModel = new WebBrowserViewModel(url);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                if (!string.IsNullOrEmpty(ViewModel.Url))
                    Web.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(ViewModel.Url)));
            }
            catch (Exception e)
            {
                AlertDialogService.ShowAlert("Unable to process request!", e.Message);
            }
        }
    }
}

