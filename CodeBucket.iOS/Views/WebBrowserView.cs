using System;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.Views
{
	public class WebBrowserView : WebView
    {
		public override void ViewDidLoad()
		{
			Title = "Web";

			base.ViewDidLoad();
			var vm = (WebBrowserViewModel)ViewModel;
            try
            {
    			if (!string.IsNullOrEmpty(vm.Url))
    				Web.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(vm.Url)));
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.ShowAlert("Unable to process request!", e.Message);
            }
		}
    }
}

