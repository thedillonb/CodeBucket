using System.Collections.Generic;
using MvvmCross.Platform;
using CodeBucket.Elements;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Core.Services;
using CodeBucket.Core.Data;
using System.Linq;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Views.Accounts
{
    public class AccountsView : ViewModelDrivenDialogViewController
    {
        public new AccountsViewModel ViewModel
        {
            get { return (AccountsViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public AccountsView()
        {
            Request = new MvxViewModelRequest { ViewModelType = typeof(AccountsViewModel) };
            Style = UITableViewStyle.Plain;
            Title = "Accounts";
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => 
                NavigationController.PushViewController(new LoginView(), true));
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.CancelButton, UIBarButtonItemStyle.Plain, 
                (s, e) => DismissViewController(true, null));
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
                t.Tapped += () =>
                {
                    if (accountsService.ActiveAccount != null && accountsService.ActiveAccount.Id == thisAccount.Id)
                        DismissViewController(true, null);
                    else
                        ViewModel.SelectAccountCommand.Execute(thisAccount);
                };

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

            var root = new RootElement(Title);
            var accountSection = new Section();
            var accounts = PopulateAccounts();
            accountSection.AddAll(accounts);
            root.Add(accountSection);
            Root = root;

            CheckEntries();
        }

        private void CheckEntries()
        {
            if (NavigationItem.LeftBarButtonItem != null)
                NavigationItem.LeftBarButtonItem.Enabled = Root.Sections.Sum(x => x.Count) > 0;
        }

		public override DialogViewController.Source CreateSizingSource(bool unevenRows)
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
            private readonly AccountsView _parent;
            public EditSource(AccountsView dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                return (indexPath.Section == 0);
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                    return UITableViewCellEditingStyle.Delete;
                return UITableViewCellEditingStyle.None;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
            {
                if (indexPath == null)
                    return;

                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        var section = _parent.Root[indexPath.Section];
                        var element = section[indexPath.Row];
                        section.Remove(element);
                        _parent.Delete(element);
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
                : base(account.Username, string.Empty, string.Empty, account.AvatarUrl)
            {
                Account = account;
            }
        }
    }
}

