using System;
using CodeBucket.Core.ViewModels.Accounts;
using System.Text;
using System.Linq;
using WebKit;
using Foundation;
using CodeBucket.Utilities;
using CodeBucket.Services;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Accounts
{
    public class LoginViewController : BasicWebViewController
    {
        // Routes that we shouldn't navigate to because apple will loose their shit because the user
        // might purchase something from a service we're not affiliated with... Assholes.
        private static string[] ForbiddenRoutes = {
            "https://bitbucket.org/features",
            "https://bitbucket.org/account/signup",
            "https://bitbucket.org/plans",
            "https://bitbucket.org/product/pricing"
        };

        public LoginViewModel ViewModel { get; } = new LoginViewModel();

        public LoginViewController()
            : base(true)
        {
            Title = "Login";

            Appearing.Take(1).Subscribe(_ => LoadRequest());

            OnActivation(d => 
            {
                this.WhenAnyObservable(x => x.ViewModel.LoginCommand.IsExecuting)
                    .SubscribeStatus("Logging in...")
                    .AddTo(d);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Fake the user-agent so that social apps don't bitch.
            Web.CustomUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A5297c Safari/602.1";
        }

        protected override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            // Fucking BitBucket and their horrible user interface.
            if (ForbiddenRoutes.Any(navigationAction.Request.Url.AbsoluteString.StartsWith))
            {
                AlertDialogService.ShowAlert("Invalid Request", "Sorry, due to restrictions, you can not sign-up for a new account in CodeBucket.");
                return false;
            }

            //We're being redirected to our redirect URL so we must have been successful
            if (navigationAction.Request.Url.Host == "codebucket")
            {
                var queryParams = navigationAction
                    .Request.Url.Query.Split('&')
	                .Select(x => x.Split('=').Select(Uri.EscapeDataString).ToArray())
	                .Where(x => x.Length >= 2)
	                .ToDictionary(x => x[0], x => x[1]);
                
                if (queryParams.TryGetValue("code", out string code))
                {
                    ViewModel.LoginCommand.Execute(code).LoggedCatch(this).Subscribe();
                    return false;
                }
            }

            //if (navigationAction.Request.Url.Path.StartsWith("/socialauth", StringComparison.Ordinal))
            //{
            //    var safari = new SFSafariViewController(new NSUrl(ViewModel.LoginUrl));
            //    PresentViewController(safari, true, null);
            //    return false;
            //}

            return base.ShouldStartLoad(webView, navigationAction);
        }

        protected override void OnLoadError(NSError error)
		{
            base.OnLoadError(error);

			//Frame interrupted error
            if (error.Code == 102 || error.Code == -999) return;
			AlertDialogService.ShowAlert("Error", "Unable to communicate with Bitbucket. " + error.LocalizedDescription);
		}

        protected override void OnLoadFinished(WKWebView webView, WKNavigation navigation)
        {
            base.OnLoadFinished(webView, navigation);

            if (webView.Url?.AbsoluteString?.StartsWith("https://bitbucket") ?? false)
            {
                var script = new StringBuilder();

                //Apple is full of clowns. The GitHub login page has links that can ultimiately end you at a place where you can purchase something
                //so we need to inject javascript that will remove these links. What a bunch of idiots...
                script.Append("$('#header').hide();");
                script.Append("$('#footer').hide();");

                Web.EvaluateJavaScript("(function(){setTimeout(function(){" + script + "}, 100); })();", null);
            }
        }

        private void LoadRequest()
        {
            //Remove all cookies & cache
            WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, NSDate.FromTimeIntervalSince1970(0), 
                () => Web.LoadRequest(new NSUrlRequest(new NSUrl(ViewModel.LoginUrl))));
        }
    }
}

