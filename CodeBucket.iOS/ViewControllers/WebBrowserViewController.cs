using System;
using CodeBucket.Services;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.ViewControllers
{
    public class WebBrowserViewController : WebView
    {
        public WebBrowserViewController()
            : base(true, true)
        {
            Title = "Web";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                var vm = ViewModel as WebBrowserViewModel;
                if (!string.IsNullOrEmpty(vm.Url))
                    Web.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(vm.Url)));
            }
            catch (Exception e)
            {
                AlertDialogService.ShowAlert("Unable to process request!", e.Message);
            }
        }
    }
}

