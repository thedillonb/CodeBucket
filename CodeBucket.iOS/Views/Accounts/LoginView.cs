using System;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.Accounts;
using UIKit;

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
			NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem(UIKit.UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
        }

		private void ShowExtraMenu()
		{
			var sheet = MonoTouch.Utilities.GetSheet("Login");
			var basicButton = sheet.AddButton("Login via BASIC");
			var cancelButton = sheet.AddButton("Cancel");
			sheet.CancelButtonIndex = cancelButton;
			sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {
				// Pin to menu
                BeginInvokeOnMainThread(() =>
                {
				if (e.ButtonIndex == basicButton)
				{
					ViewModel.GoToOldLoginWaysCommand.Execute(null);
				}
                });
			};

			sheet.ShowInView(this.View);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (ViewModel.AttemptedAccount != null)
				LoadRequest();
		}

		protected override bool ShouldStartLoad(Foundation.NSUrlRequest request, UIKit.UIWebViewNavigationType navigationType)
        {
            //We're being redirected to our redirect URL so we must have been successful
            if (request.Url.Host == "dillonbuchanan.com")
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

            //Inject some Javascript so we can set the username if there is an attempted account
			if (ViewModel.AttemptedAccount != null)
            {
				var script = "(function() { setTimeout(function() { $('input[name=\"login\"]').val('" + ViewModel.AttemptedAccount.Username + "').attr('readonly', 'readonly'); }, 100); })();";
                Web.EvaluateJavascript(script);
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

