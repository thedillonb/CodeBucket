using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch;
using BitbucketBrowser.Controllers.Accounts;
using BitbucketBrowser.Data;

namespace BitbucketBrowser.Controllers
{
    public class SettingsController : BaseDialogViewController
    {
		private Section _accountOptionsSection;

        public SettingsController()
            : base(false)
        {
            Title = "Settings";
            Style = UITableViewStyle.Grouped;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => DismissModalViewControllerAnimated(true));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
			var root = new RootElement(Title);

            var accountSection = new Section("Accounts");
			root.Add(accountSection);
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new StyledElement(thisAccount.Username, thisAccount.AccountType.ToString(), UITableViewCellStyle.Subtitle) { Image = Images.Anonymous };
                if (!string.IsNullOrEmpty(thisAccount.AvatarUrl))
                    t.ImageUri = new Uri(thisAccount.AvatarUrl);

                t.Tapped += () => { 

					//Change the user delegate
					Action<Account> changeUserAction = (a) => {
						Application.SetUser(a);
						DismissModalViewControllerAnimated(true);
					};

					//If the account doesn't remember the password we need to prompt
					if (thisAccount.DontRemember)
					{
						if (thisAccount.AccountType == BitbucketBrowser.Data.Account.Type.Bitbucket)
						{
							var loginController = new Bitbucket.Controllers.Accounts.LoginViewController() { Username = thisAccount.Username };
							loginController.LoginComplete = changeUserAction;
							NavigationController.PushViewController(loginController, true);
						}
					}
					//Change the user!
					else
					{
						changeUserAction(thisAccount);
					}
                };
                
                
                if (Application.Account != null && string.Compare(account.Username, Application.Account.Username, true) == 0)
                {
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                }
                
                accountSection.Add(t);
            }

            var addAccount = new StyledElement("Add Account", () => {
				var ctrl = new AddAccountController();
				ctrl.AccountAdded = (a) => NavigationController.PopToViewController(this, true);
				NavigationController.PushViewController(ctrl, true);
			});
            //addAccount.Image = Images.CommentAdd;
            accountSection.Add(addAccount);

			var currentAccount = Application.Account;
			if (currentAccount != null)
			{
				_accountOptionsSection = new Section("Account Options");
				root.Add(_accountOptionsSection);
	            var autoSigninElement = new BitbucketBrowser.Elements.TrueFalseElement("Remember Credentials", !currentAccount.DontRemember);
	            autoSigninElement.ValueChanged += (sender, e) => { 
					currentAccount.DontRemember = !autoSigninElement.Value; 
					currentAccount.Update();
				};
				_accountOptionsSection.Add(autoSigninElement);
			}

            var supportSection = new Section("Support");
			root.Add (supportSection);
            supportSection.Add(new StyledElement("Technical Support", () => {
                var web = new BitbucketBrowser.Controllers.HelpViewController();
                NavigationController.PushViewController(web, true);
            }));

            if (MFMailComposeViewController.CanSendMail)
                supportSection.Add(new StyledElement("Contact Me", OpenMailer));


            var aboutSection = new Section("About", "Thank you for downloading. Enjoy!");
			root.Add(aboutSection);
            aboutSection.Add(new StyledElement("Follow On Twitter", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp"))));
            aboutSection.Add(new StyledElement("Rate This App", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8"))));
            aboutSection.Add(new StyledElement("App Version", NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString()));

			//Assign the root
			Root = root;
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
			string currentUsername = null;
			if (Application.Account != null)
				currentUsername = Application.Account.Username;

			//Remove the designated username
            Application.Accounts.Remove(username);

			if ((Application.Account != null && String.Compare(currentUsername, username, true) == 0) || Application.Accounts.Count == 0) 
			{
                NavigationItem.LeftBarButtonItem.Enabled = false;
				Root.Remove(_accountOptionsSection);
				Application.SetUser(null);
			}
        }

        private void OpenMailer()
        {
            var mailer = new MFMailComposeViewController();
            mailer.SetSubject("CodeBucket Feedback");
            mailer.SetToRecipients(new string[] { "codebucketapp@gmail.com" });
            mailer.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
            mailer.Finished += (sender, e) => this.DismissModalViewControllerAnimated(true);
            this.PresentModalViewController(mailer, true);
        }

        private class EditSource : Source
        {
            private readonly SettingsController _parent;
            public EditSource(SettingsController dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return (indexPath.Section == 0 && indexPath.Row != (_parent.Root[0].Count - 1));
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                if (indexPath.Section == 0 && indexPath.Row != (_parent.Root[0].Count - 1))
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
    }
}


