using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeBucket.Data;
using CodeFramework.Elements;
using CodeFramework.Controllers;
using CodeFramework.Views;
using System.Linq;

namespace CodeBucket.Controllers
{
	/// <summary>
	/// A list of the accounts that are currently listed with the application
	/// </summary>
	public class AccountsController : CodeFramework.Controllers.AccountsController<Account>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsController"/> class.
		/// </summary>
		public AccountsController ()
			: base(Application.Accounts)
		{
		}

        protected override UIViewController AddAccount()
        {
            var ctrl = new Bitbucket.Controllers.Accounts.LoginViewController();
            ctrl.Login = (username, password) => {
                Utils.Login.LoginAccount(username, password, ctrl);
            };
            return ctrl;
        }

        protected override void AccountSelected(Account account)
        {
            //If the account doesn't remember the password we need to prompt
            if (account.DontRemember)
            {
                var loginController = new CodeBucket.Bitbucket.Controllers.Accounts.LoginViewController() { Username = account.Username };
                loginController.Login = (username, password) => {
                    Utils.Login.LoginAccount(username, password, loginController);
                };
                NavigationController.PushViewController(loginController, true);
            }
            //Change the user!
            else
            {
                Utils.Login.LoginAccount(account.Username, account.Password, this);
            }
        }
    }
}

