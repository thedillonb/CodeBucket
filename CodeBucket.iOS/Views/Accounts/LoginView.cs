using System;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.Accounts;
using UIKit;
using CodeBucket.Utils;
using System.Text;
using System.Linq;

namespace CodeBucket.Views.Accounts
{
    public class LoginView : WebView
    {
        // Routes that we shouldn't navigate to because apple will loose their shit because the user
        // might purchase something from a service we're not affiliated with... Assholes.
        private static string[] ForbiddenRoutes = {
            "https://bitbucket.org/features",
            "https://bitbucket.org/account/signup",
            "https://bitbucket.org/plans",
            "https://bitbucket.org/product/pricing",
        };

		public new LoginViewModel ViewModel
		{
			get { return (LoginViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public LoginView()
            : base(true)
        {
            Request = new Cirrious.MvvmCross.ViewModels.MvxViewModelRequest { ViewModelType = typeof(LoginViewModel) };
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
            if (ForbiddenRoutes.Any(request.Url.AbsoluteString.StartsWith))
            {
                MonoTouch.Utilities.ShowAlert("Invalid Request", "Sorry, due to restrictions you can not sign-up for a new account in CodeBucket.");
                return false;
            }

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

        protected override void OnLoadFinished(object sender, EventArgs e)
        {
            base.OnLoadFinished(sender, e);

            if (Web.Request?.Url?.AbsoluteString?.StartsWith("https://bitbucket") ?? false)
            {
                var script = new StringBuilder();

                //Apple is full of clowns. The GitHub login page has links that can ultimiately end you at a place where you can purchase something
                //so we need to inject javascript that will remove these links. What a bunch of idiots...
                script.Append("$('#header').hide();");
                script.Append("$('#footer').hide();");

                Web.EvaluateJavascript("(function(){setTimeout(function(){" + script + "}, 100); })();");
            }
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

