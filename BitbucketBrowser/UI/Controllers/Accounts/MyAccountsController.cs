using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI.Controllers.Accounts
{
    public class MyAccountsController : BaseDialogViewController
    {
        public MyAccountsController()
            : base(false)
        {
            Title = "Accounts";
            Style = UITableViewStyle.Plain;

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => NavigationController.PushViewController(new LoginViewController(), true));
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => DismissModalViewControllerAnimated(true));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var s = new Section();
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new StyledElement(thisAccount.Username);
                t.Tapped += () => { 
                    Application.SetUser(thisAccount);
                    DismissModalViewControllerAnimated(true);
                    
                };
                
                
                if (string.Compare(account.Username, Application.Account.Username, true) == 0)
                {
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                }
                
                s.Add(t);
            }

            TableView.TableFooterView.Hidden = s.Count == 0;
            Root = new RootElement(Title) { s };
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new EditSource(this);
        }

        private void Delete(Element element)
        {
            var styledElement = element as StyledElement;
            if (styledElement == null)
                return;

            var username = styledElement.Caption;
            Application.Accounts.Remove(username);

            if (Application.Accounts.Count == 0)
            {
                TableView.TableFooterView.Hidden = true;
            }

            if ((Application.Accounts.Count == 0) ||
                (String.Compare(Application.Account.Username, username, true) == 0))
            {
                //Block the ability to go back!
                NavigationItem.LeftBarButtonItem.Enabled = false;
                return;
            }
        }

        private class EditSource : Source
        {
            private readonly MyAccountsController _parent;
            public EditSource(MyAccountsController dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return true;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
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
    }
}


