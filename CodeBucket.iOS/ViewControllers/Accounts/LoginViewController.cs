using System;
using System.Linq;
using Foundation;
using System.Reactive.Linq;
using UIKit;

namespace CodeBucket.ViewControllers.Accounts
{
    public class LoginViewController : BaseViewController
    {
        public LoginViewController()
        {
            Title = "Login";

            var id = Core.Secrets.ClientId;
            var loginUrl = new NSUrl(string.Format("https://bitbucket.org/site/oauth2/authorize?client_id={0}&response_type=code", id));

            Appeared.Take(1).Subscribe(_ =>
            {
                UIApplication.SharedApplication.OpenUrl(loginUrl);
            });
        }
    }
}

