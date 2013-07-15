using System;
using CodeBucket.Data;
using MonoTouch.UIKit;
using CodeFramework.Controllers;
using System.Linq;
using MonoTouch;

namespace CodeBucket.Utils
{
    public class Login
    {
        public static void LoginAccount(string user, string pass, UIViewController ctrl, Action error = null)
        {
            //Does this user exist?
            var account = Application.Accounts.Find(user);
            var exists = account != null;
            if (!exists)
                account = new Account { Username = user, Password = pass };

            ctrl.DoWork("Logging in...", () => {

                var client = new BitbucketSharp.Client(user, pass) { Timeout = 30 * 1000 };
                var privileges = client.Account.GetPrivileges();
                var userInfo = client.Account.GetInfo();

                account.FullName = (userInfo.User.FirstName ?? string.Empty) + " " + (userInfo.User.LastName ?? string.Empty);
                account.Username = userInfo.User.Username;
                account.AvatarUrl = userInfo.User.Avatar;
                if (privileges != null && privileges.Teams != null)
                {
                    account.Teams = privileges.Teams.Keys.ToList();
                    account.Teams.Remove(account.Username);
                }

                if (exists)
                    account.Update();
                else
                    Application.Accounts.Insert(account);

                Application.SetUser(account);
                ctrl.InvokeOnMainThread(TransitionToSlideout);

            }, (ex) => {
                //If there is a login failure, unset the user
                Application.SetUser(null);
                Utilities.ShowAlert("Unable to Authenticate", "Unable to login as user " + account.Username + ". Please check your credentials and try again. Remember, credentials are case sensitive!");
                if (error != null)
                    error();
            });
        }

        public static void TransitionToController(UIViewController controller)
        {
            Transition(controller, UIViewAnimationOptions.TransitionCrossDissolve);
        }

        private static void TransitionToSlideout()
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var controller = new CodeBucket.Controllers.SlideoutNavigationController();
            if (appDelegate != null)
                appDelegate.Slideout = controller;
            Transition(controller, UIViewAnimationOptions.TransitionFlipFromRight);
        }

        public static void Transition(UIViewController controller, UIViewAnimationOptions options)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            UIView.Transition(window, 1.0, options, () => {
                var oldState = UIView.AnimationsEnabled;
                UIView.AnimationsEnabled = false;
                window.RootViewController = controller;
                UIView.AnimationsEnabled = oldState;
            }, null);
        }
    }
}

