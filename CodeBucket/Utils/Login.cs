using System;
using CodeBucket.Data;
using MonoTouch.UIKit;
using CodeFramework.Controllers;
using System.Linq;
using MonoTouch;
using CodeFramework.Utils;

namespace CodeBucket.Utils
{
    public class Login
    {
        public static void LoginAccount(string user, string pass, UIViewController ctrl, Action<Exception> error = null)
        {
            //Does this user exist?
            var account = Application.Accounts.Find(user);
            var exists = account != null;
            if (!exists)
                account = new Account { Username = user, Password = pass };

            ctrl.DoWork("Logging in...", () => {
                BitbucketSharp.Models.UsersModel userInfo;

                try
                {
                    var client = new BitbucketSharp.Client(user, pass) { Timeout = 30 * 1000 };
                    userInfo = client.Account.GetInfo();
                }
                catch (Exception)
                {
                    throw new Exception("Unable to login as user " + account.Username + ". Please check your credentials and try again. Remember, credentials are case sensitive!");
                }

                account.FullName = (userInfo.User.FirstName ?? string.Empty) + " " + (userInfo.User.LastName ?? string.Empty);
                account.Username = userInfo.User.Username;
                account.Password = pass;
                account.AvatarUrl = userInfo.User.Avatar;

                if (exists)
                    Application.Accounts.Update(account);
                else
                    Application.Accounts.Insert(account);

                Application.SetUser(account);
                ctrl.InvokeOnMainThread(TransitionToSlideout);

            }, (ex) => {
                Console.WriteLine(ex.Message);

                //If there is a login failure, unset the user
                Application.SetUser(null);

                //Show an alert and trigger the callback when the user dismisses it
                Utilities.ShowAlert("Unable to Authenticate", ex.Message, () => {
                    if (error != null)
                        error(ex);
                });
            });
        }

        private static void TransitionToSlideout()
        {
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            var controller = new CodeBucket.ViewControllers.SlideoutNavigationViewController();
            if (appDelegate != null)
                appDelegate.Slideout = controller;
            Transitions.Transition(controller, UIViewAnimationOptions.TransitionFlipFromRight);
        }
    }
}

