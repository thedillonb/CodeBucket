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
            Model = Application.Accounts;
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
            Application.Accounts.ForEach(x => {
                var t = new StyledElement(x.Username);
                t.Tapped += () => { 

                    Application.SetUser(x);
                    this.DismissModalViewControllerAnimated(true);

                };

                s.Add(t);
            });

            Root = new RootElement(Title) { s };
        }

        protected override List<Account> OnUpdate ()
        {
            return Model;
        }
    }
}


namespace BitbucketBrowser 
{
    public class Account
    {
        public string Username;
        public string Password;
    }
}

