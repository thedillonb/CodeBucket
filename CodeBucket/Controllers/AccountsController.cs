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

        protected override void Populate()
        {
            var accountSection = new Section();
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new AccountElement(thisAccount);
                t.Tapped += () => { 
                    //If the account doesn't remember the password we need to prompt
                    if (thisAccount.DontRemember)
                    {
                        var loginController = new CodeBucket.Bitbucket.Controllers.Accounts.LoginViewController() { Username = thisAccount.Username };
                        loginController.Login = (username, password) => {
                            Utils.Login.LoginAccount(username, password, loginController);
                        };
                        NavigationController.PushViewController(loginController, true);
                    }
                    //Change the user!
                    else
                    {
                        Utils.Login.LoginAccount(thisAccount.Username, thisAccount.Password, this);
                    }
                };

                //Check to see if this account is the active account. Application.Account could be null 
                //so make it the target of the equals, not the source.
                if (thisAccount.Equals(Application.Account))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                accountSection.Add(t);
            }

            var addAccountSection = new Section();
            var addAccount = new StyledElement("Add Account", () => {
                var ctrl = new Bitbucket.Controllers.Accounts.LoginViewController();
                ctrl.Login = (username, password) => {
                    Utils.Login.LoginAccount(username, password, ctrl);
                };
                NavigationController.PushViewController(ctrl, true);
            });
            //addAccount.Image = Images.CommentAdd;
            addAccountSection.Add(addAccount);


            Root = new RootElement(Title) { accountSection, addAccountSection };
        }
    }
}

