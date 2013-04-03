using System;
using CodeFramework.UI.Controllers;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;
using BitbucketBrowser.Data;

namespace BitbucketBrowser.Controllers.Accounts
{
	/// <summary>
	/// A list of the accounts that are currently listed with the application
	/// </summary>
	public class AccountsController : BaseDialogViewController
	{
		/// <summary>
		/// Occurs when account selected.
		/// </summary>
		public event Action<Account> AccountSelected;

		/// <summary>
		/// Raises the account selected event.
		/// </summary>
		/// <param name="account">Account.</param>
		protected virtual void OnAccountSelected(Account account)
		{
			var d = AccountSelected;
			if (d != null)
				d(account);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BitbucketBrowser.Controllers.Accounts.AccountsController"/> class.
		/// </summary>
		public AccountsController ()
			: base(true, "Accounts")
		{
			Title = "Accounts";
			Style = UITableViewStyle.Grouped;
		}

		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			var accountSection = new Section();
			foreach (var account in Application.Accounts)
			{
				var thisAccount = account;
				var t = new StyledElement(thisAccount.Username) { Image = Images.Anonymous };
				if (!string.IsNullOrEmpty(thisAccount.AvatarUrl))
					t.ImageUri = new Uri(thisAccount.AvatarUrl);
				
				t.Tapped += () => { 
					if (thisAccount.DontRemember)
					{
						var loginController = new LoginViewController() { Username = thisAccount.Username };
						loginController.LoginComplete = () => {
							OnAccountSelected(thisAccount);
						};
						NavigationController.PushViewController(loginController, true);
					}
					else
					{
						OnAccountSelected(thisAccount);
					}
				};
				
				accountSection.Add(t);
			}

			var addSection = new Section();
			var addAccount = new StyledElement("Add Account", () => NavigationController.PushViewController(new LoginViewController(), true));
			//addAccount.Image = Images.CommentAdd;
			addSection.Add(addAccount);

			Root = new RootElement(Title) { accountSection, addSection };
		}
	}
}

