using System;
using MonoTouch.UIKit;
using CodeBucket.Data;
using System.Linq;
using CodeFramework.Controllers;
using CodeBucket.Bitbucket.Controllers.Accounts;

namespace CodeBucket.Controllers
{
    public class StartupController : UIViewController
    {
        private UIImageView _imgView;

        public StartupController()
        {
            WantsFullScreenLayout = true;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            _imgView.Frame = this.View.Bounds;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIImage img;

            //Load the background image
            img = MonoTouch.Utilities.IsTall ? 
                    UIImageHelper.FromFileAuto("Default-568h", "png") : 
                    UIImageHelper.FromFileAuto("Default", "png");

            _imgView = new UIImageView(img);
            Add(_imgView);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //Start the login
            ProcessAccounts();
        }

        
        /// <summary>
        /// Processes the accounts.
        /// </summary>
        private void ProcessAccounts()
        {
            var defaultAccount = GetDefaultAccount();

            //There's no accounts... or something bad has happened to the default
            if (Application.Accounts.Count == 0 || defaultAccount == null)
            {
                var login = new LoginViewController();
                login.Login = (username, password) => {
                    Utils.Login.LoginAccount(username, password, login);
                };
                Utils.Login.TransitionToController(login);
                return;
            }

            //Don't remember, prompt for password
            if (defaultAccount.DontRemember)
            {
                var accountsController = new AccountsController();
                accountsController.NavigationItem.LeftBarButtonItem = null;
                var login = new LoginViewController { Username = defaultAccount.Username };
                login.Login = (username, password) => {
                    Utils.Login.LoginAccount(username, password, login);
                };

                var navigationController = new UINavigationController(accountsController);
                navigationController.PushViewController(login, false);
                Utils.Login.TransitionToController(navigationController);
            }
            //If the user wanted to remember the account
            else
            {
                Utils.Login.LoginAccount(defaultAccount.Username, defaultAccount.Password, this);
            }
        }

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        private Account GetDefaultAccount()
        {
            var defaultAccount = Application.Accounts.GetDefault();
            if (defaultAccount == null)
            {
                defaultAccount = Application.Accounts.FirstOrDefault();
                Application.Accounts.SetDefault(defaultAccount);
            }
            return defaultAccount;
        }
    }
}

