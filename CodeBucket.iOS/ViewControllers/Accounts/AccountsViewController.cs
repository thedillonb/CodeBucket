using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.DialogElements;
using CodeBucket.ViewControllers;
using MvvmCross.Platform;
using UIKit;

namespace CodeBucket.ViewControllers.Accounts
{
    public class AccountsViewController : DialogViewController
    {
        public AccountsViewModel ViewModel { get; }

        public AccountsViewController()
            : base(style: UITableViewStyle.Plain)
        {
            ViewModel = new AccountsViewModel(Mvx.Resolve<IAccountsService>());
            Title = "Accounts";

            var add = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var cancel = NavigationItem.LeftBarButtonItem = new UIBarButtonItem { Image = Images.Buttons.Cancel };

            OnActivation(d =>
            {
                d(add.GetClickedObservable().BindCommand(ViewModel.AddAccountCommand));
                d(cancel.GetClickedObservable().BindCommand(ViewModel.DismissCommand));
                d(ViewModel.AddAccountCommand.Subscribe(_ => NavigationController.PushViewController(new LoginViewController(), true)));
                d(ViewModel.DismissCommand.Subscribe(_ => DismissViewController(true, null)));
            });
        }

        /// <summary>
        /// Called when the accounts need to be populated
        /// </summary>
        /// <returns>The accounts.</returns>
        protected List<AccountElement> PopulateAccounts()
        {
            var accounts = new List<AccountElement>();
            var accountsService = Mvx.Resolve<IAccountsService>();

            foreach (var account in accountsService)
            {
                var thisAccount = account;
                var t = new AccountElement(thisAccount);
                t.Clicked.Select(x => thisAccount).BindCommand(ViewModel.SelectAccountCommand);

                //Check to see if this account is the active account. Application.Account could be null 
                //so make it the target of the equals, not the source.
                if (thisAccount.Equals(accountsService.ActiveAccount))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                accounts.Add(t);
            }
            return accounts;
        }

        /// <summary>
        /// Called when an account is deleted
        /// </summary>
        /// <param name="account">Account.</param>
        protected void AccountDeleted(BitbucketAccount account)
        {
            //Remove the designated username
            var thisAccount = account;
            var accountsService = Mvx.Resolve<IAccountsService>();

            accountsService.Remove(thisAccount);

            if (accountsService.ActiveAccount != null && accountsService.ActiveAccount.Equals(thisAccount))
            {
                accountsService.SetActiveAccount(null);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Root.Reset(new Section { PopulateAccounts() });
            CheckEntries();
        }

        private void CheckEntries()
        {
            if (NavigationItem.LeftBarButtonItem != null)
            {
                var accountsService = Mvx.Resolve<IAccountsService>();
                NavigationItem.LeftBarButtonItem.Enabled = accountsService.ActiveAccount != null;
            }
        }

		public override DialogViewController.Source CreateSizingSource()
        {
            return new EditSource(this);
        }

        private void Delete(Element element)
        {
            var accountElement = element as AccountElement;
            if (accountElement == null)
                return;

            //Remove the designated username
            AccountDeleted(accountElement.Account);

            CheckEntries();
        }

		private class EditSource : DialogViewController.Source
        {
            private readonly WeakReference<AccountsViewController> _parent;
            public EditSource(AccountsViewController dvc) 
                : base (dvc)
            {
                _parent = new WeakReference<AccountsViewController>(dvc);
            }

            public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                return (indexPath.Section == 0);
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                return indexPath.Section == 0 ? UITableViewCellEditingStyle.Delete : UITableViewCellEditingStyle.None;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
            {
                if (indexPath == null)
                    return;

                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        var section = Root?[indexPath.Section];
                        var element = section?[indexPath.Row];
                        section?.Remove(element);
                        _parent.Get()?.Delete(element);
                        break;
                }
            }
        }

        /// <summary>
        /// An element that represents an account object
        /// </summary>
        protected class AccountElement : UserElement
        {
            public BitbucketAccount Account { get; private set; }

            public AccountElement(BitbucketAccount account)
                : base(new UserItemViewModel(account.Username, null, new Avatar(account.AvatarUrl)))
            {
                Account = account;
            }
        }
    }
}

