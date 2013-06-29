using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using CodeBucket.Data;
using CodeBucket.Elements;

namespace CodeBucket.Controllers
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
				var t = new AccountElement(thisAccount);
                t.Tapped += () => { 

					//Change the user delegate
					Action<Account> changeUserAction = (a) => {
						Application.SetUser(a);
						DismissModalViewControllerAnimated(true);
					};

					//If the account doesn't remember the password we need to prompt
					if (thisAccount.DontRemember)
					{
						if (thisAccount.AccountType == Account.Type.Bitbucket)
						{
							var loginController = new CodeBucket.Bitbucket.Controllers.Accounts.LoginViewController() { Username = thisAccount.Username };
							loginController.LoginComplete = changeUserAction;
							NavigationController.PushViewController(loginController, true);
						}
                        else if (thisAccount.AccountType == Account.Type.GitHub)
                        {
                            var loginController = new CodeBucket.GitHub.Controllers.Accounts.GitHubLoginController() { Username = thisAccount.Username };
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
                
				//Check to see if this account is the active account. Application.Account could be null 
				//so make it the target of the equals, not the source.
                if (thisAccount.Equals(Application.Account))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
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
	            var autoSigninElement = new TrueFalseElement("Remember Credentials", !currentAccount.DontRemember);
	            autoSigninElement.ValueChanged += (sender, e) => { 
					currentAccount.DontRemember = !autoSigninElement.Value; 
					currentAccount.Update();
				};
				_accountOptionsSection.Add(autoSigninElement);
			}

            var supportSection = new Section("Support");
			root.Add (supportSection);
            supportSection.Add(new StyledElement("Feedback & Support", () => {
                OpenUserVoice();
            }));

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
			var accountElement = element as AccountElement;
			if (accountElement == null)
                return;

			var account = accountElement.Account;
			var username = account.Username;
			string currentUsername = null;
			if (Application.Account != null)
				currentUsername = Application.Account.Username;

			//Remove the designated username
            Application.Accounts.Remove(account);

			if (Application.Account.Equals(account))
			{
                NavigationItem.LeftBarButtonItem.Enabled = false;
				Root.Remove(_accountOptionsSection);
				Application.SetUser(null);
			}
        }

        private void OpenUserVoice()
        {
            var config = UserVoice.UVConfig.Create("http://codebucket.uservoice.com", "pnuDmPENErDiDpXrms1DTg", "iDboMdCIwe2E5hJFa8hy9K9I5wZqnjKCE0RPHLhZIk");
            UserVoice.UserVoice.PresentUserVoiceInterface(this, config);
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

        /// <summary>
        /// An element that represents an account object
        /// </summary>
		private class AccountElement : StyledElement
		{
			public Account Account { get; private set; }
			public AccountElement(Account account)
				: base(account.Username, account.AccountType.ToString(), UITableViewCellStyle.Subtitle)
			{
				Account = account;
				Image = Images.Anonymous;
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
                    TextLabel.Frame = new System.Drawing.RectangleF(42, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                    DetailTextLabel.Frame = new System.Drawing.RectangleF(42, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
                }
            }
		}
    }
}


