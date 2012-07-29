using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using BitbucketSharp.Models;

namespace BitbucketBrowser.UI
{
    public class MyAccountsController : Controller<List<Account>>
    {
        public MyAccountsController()
            : base (false, false)
        {
            Title = "Accounts";
            Model = new List<Account>(Application.Accounts);
            Style = UITableViewStyle.Plain;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Pull the model!

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => {  
                NavigationController.PushViewController(new LoginViewController(), true);
            });

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => {  
                this.DismissModalViewControllerAnimated(true);
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Refresh(false);
        }

        protected override void OnRefresh ()
        {
            var s = new Section();

            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new StyledElement(thisAccount.Username);
                t.Tapped += () => { 
                    Application.SetUser(thisAccount);
                    this.DismissModalViewControllerAnimated(true);

                };

                s.Add(t);
            };

            TableView.TableFooterView.Hidden = false;
            Root = new RootElement(Title) { s };
        }

        protected override List<Account> OnUpdate ()
        {
            return Model;
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

        private class EditSource : DialogViewController.Source
        {
            private MyAccountsController _parent;
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


