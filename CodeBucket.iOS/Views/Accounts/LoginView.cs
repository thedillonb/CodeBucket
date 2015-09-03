using System;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.Accounts;
using UIKit;
using CodeBucket.Utils;

namespace CodeBucket.Views.Accounts
{
    public class LoginView : WebView
    {
		public new LoginViewModel ViewModel
		{
			get { return (LoginViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public LoginView()
            : base(false)
        {
            Title = "Login";
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			LoadRequest();

            var hud = this.CreateHud();

            ViewModel.Bind(x => x.IsLoggingIn, x =>
            {
                if (x)
                {
                    hud.Show("Logging in...");
                }
                else
                {
                    hud.Hide();
                }
            });
		}

		protected override bool ShouldStartLoad(Foundation.NSUrlRequest request, UIKit.UIWebViewNavigationType navigationType)
        {
            // Fucking BitBucket and their horrible user interface.
            if (request.Url.AbsoluteString == "https://bitbucket.org/features")
                return false;

            //We're being redirected to our redirect URL so we must have been successful
            if (request.Url.Host == "codebucket")
            {
                var code = request.Url.Query.Split('=')[1];
				ViewModel.Login(code);
                return false;
            }
            return base.ShouldStartLoad(request, navigationType);
        }

		protected override void OnLoadError(object sender, UIWebErrorArgs e)
		{
			base.OnLoadError(sender, e);

			//Frame interrupted error
			if (e.Error.Code == 102)
				return;

			MonoTouch.Utilities.ShowAlert("Error", "Unable to communicate with Bitbucket. " + e.Error.LocalizedDescription);
		}

        private void LoadRequest()
        {
            //Remove all cookies & cache
            foreach (var c in Foundation.NSHttpCookieStorage.SharedStorage.Cookies)
                Foundation.NSHttpCookieStorage.SharedStorage.DeleteCookie(c);
            Foundation.NSUrlCache.SharedCache.RemoveAllCachedResponses();
			Web.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(ViewModel.LoginUrl)));
        }
    }
}

