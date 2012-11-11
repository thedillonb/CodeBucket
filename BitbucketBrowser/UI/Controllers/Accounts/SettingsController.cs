using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch;

namespace BitbucketBrowser.UI.Controllers.Accounts
{
    public class SettingsController : BaseDialogViewController
    {
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

            var accountSection = new Section("Accounts");
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new StyledElement(thisAccount.Username) { Image = Images.Anonymous };
                if (!string.IsNullOrEmpty(thisAccount.AvatarUrl))
                    t.ImageUri = new Uri(thisAccount.AvatarUrl);

                t.Tapped += () => { 
                    Application.SetUser(thisAccount);
                    DismissModalViewControllerAnimated(true);
                    
                };
                
                
                if (string.Compare(account.Username, Application.Account.Username, true) == 0)
                {
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                }
                
                accountSection.Add(t);
            }

            var addAccount = new StyledElement("Add Account", () => NavigationController.PushViewController(new LoginViewController(), true));
            //addAccount.Image = Images.CommentAdd;
            accountSection.Add(addAccount);

            var supportSection = new Section("Support");
            supportSection.Add(new StyledElement("Technical Support", () => {
                var web = new WebViewController(true) { Title = "Help" };
                web.Web.LoadRequest(new NSUrlRequest(new NSUrl("http://support.codebucket.dillonbuchanan.com")));
                NavigationController.PushViewController(web, true);
            }));

            if (MFMailComposeViewController.CanSendMail)
                supportSection.Add(new StyledElement("Contact Me", OpenMailer));


            var aboutSection = new Section("About", "Thank you for downloading. Enjoy!");
            aboutSection.Add(new StyledElement("Follow On Twitter", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp"))));
            aboutSection.Add(new StyledElement("App Version", NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString()));

            //TableView.TableFooterView.Hidden = s.Count == 0;
            Root = new RootElement(Title) { accountSection, supportSection, aboutSection };
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


