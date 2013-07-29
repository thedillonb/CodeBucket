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
    public class AccountsController : BaseDialogViewController
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        public AccountsController ()
            : base(true, "Accounts")
        {
            Title = "Accounts";
            Style = UITableViewStyle.Grouped;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Cancel, () => this.DismissViewController(true, null)));
        }

        /// <summary>
        /// Views the will appear.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Populate();
        }

        private void Populate()
        {
            var accountSection = new Section();
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new AccountElement(thisAccount);
                t.Tapped += () => { 
                    AccountSelected(thisAccount);
                };

                //Check to see if this account is the active account. Application.Account could be null 
                //so make it the target of the equals, not the source.
                if (thisAccount.Equals(Application.Account))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                accountSection.Add(t);
            }

            var addAccountSection = new Section();
            var addAccount = new StyledElement("Add Account", () => NavigationController.PushViewController(AddAccount(), true));
            //addAccount.Image = Images.CommentAdd;
            addAccountSection.Add(addAccount);

            Root = new RootElement(Title) { accountSection, addAccountSection };
        }

        
        private UIViewController AddAccount()
        {
            var ctrl = new Bitbucket.Controllers.Accounts.LoginViewController();
            ctrl.Login = (username, password) => {
                Utils.Login.LoginAccount(username, password, ctrl);
            };
            return ctrl;
        }

        private void AccountSelected(Account account)
        {
            var a = account as Account;
            //If the account doesn't remember the password we need to prompt
            if (a.DontRemember)
            {
                DoLoginStuff(account.Username);
            }
            //Change the user!
            else
            {
                Utils.Login.LoginAccount(account.Username, a.Password, this, (ex) => {
                    DoLoginStuff(account.Username);
                });
            }
        }

        private void DoLoginStuff(string user)
        {
            var loginController = new CodeBucket.Bitbucket.Controllers.Accounts.LoginViewController() { Username = user };
            loginController.Login = (username, password) => {
                Utils.Login.LoginAccount(username, password, loginController);
            };
            NavigationController.PushViewController(loginController, true);
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new EditSource(this);
        }

        private void Delete(Element element)
        {
            var accountElement = element as AccountElement;
            if (accountElement == null)
                return;

            //Remove the designated username
            var account = accountElement.Account;
            Application.Accounts.Remove(account);

            if (Application.Account != null && Application.Account.Equals(account))
            {
                NavigationItem.LeftBarButtonItem.Enabled = false;
                Application.Account = null;
            }
        }

        private class EditSource : MonoTouch.Dialog.DialogViewController.Source
        {
            private readonly AccountsController _parent;
            public EditSource(AccountsController dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return (indexPath.Section == 0);
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                    return UITableViewCellEditingStyle.Delete;
                return UITableViewCellEditingStyle.None;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        var section = _parent.Root[indexPath.Section];
                        var element = section[indexPath.Row];
                        _parent.Delete(element);
                        section.Remove(element);
                        break;
                }
            }
        }

        /// <summary>
        /// An element that represents an account object
        /// </summary>
        protected class AccountElement : StyledElement
        {
            public Account Account { get; private set; }
            public AccountElement(Account account)
                : base(account.Username)
            {
                Account = account;
                Image = CodeFramework.Images.Misc.Anonymous;
                if (!string.IsNullOrEmpty(Account.AvatarUrl))
                    this.ImageUri = new Uri(Account.AvatarUrl);
            }

            // We need to create our own cell so we can position the image view appropriately
            protected override UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
            {
                return new PinnedImageTableViewCell(style, key);
            }

            /// <summary>
            /// This class is to make sure the imageview is of a specific size... :(
            /// </summary>
            private class PinnedImageTableViewCell : UITableViewCell
            {
                public PinnedImageTableViewCell(UITableViewCellStyle style, string key) : base(style, key) { }

                public override void LayoutSubviews()
                {
                    base.LayoutSubviews();
                    ImageView.Frame = new System.Drawing.RectangleF(5, 5, 32, 32);
                    ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
                    ImageView.Layer.CornerRadius = 4.0f;
                    ImageView.Layer.MasksToBounds = true;
                    TextLabel.Frame = new System.Drawing.RectangleF(42, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                    if (DetailTextLabel != null)
                        DetailTextLabel.Frame = new System.Drawing.RectangleF(42, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
                }
            }
        }
    }
}

