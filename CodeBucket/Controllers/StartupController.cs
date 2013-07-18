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
        private UIImage _img;

        public StartupController()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                WantsFullScreenLayout = true;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (_imgView != null)
                _imgView.Frame = this.View.Bounds;

            if (_img != null)
                _img.Dispose();
            _img = null;

            //Load the background image
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                _img = UIImageHelper.FromFileAuto(MonoTouch.Utilities.IsTall ? "Default-568h" : "Default");
            }
            else
            {
                if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                    _img = UIImageHelper.FromFileAuto("Default-Portrait");
                else
                    _img = UIImageHelper.FromFileAuto("Default-Landscape");
            }

            if (_img != null)
                _imgView.Image = _img;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;
            _imgView = new UIImageView();
            _imgView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
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
                Utils.Transitions.TransitionToController(login);
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
                Utils.Transitions.TransitionToController(navigationController);
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

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
            return UIInterfaceOrientationMask.All;
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                    return true;
                return false;
            }
            return true;
        }
    }
}

