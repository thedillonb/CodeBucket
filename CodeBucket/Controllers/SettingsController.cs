using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using CodeBucket.Data;
using CodeFramework.Elements;
using CodeFramework.Controllers;

namespace CodeBucket.Controllers
{
    public class SettingsController : BaseDialogViewController
    {
        public SettingsController()
            : base(false)
        {
            Title = "Settings";
            Style = UITableViewStyle.Grouped;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => DismissViewController(true, null));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var root = new RootElement(Title);
			var currentAccount = Application.Account;

            root.Add(new Section(string.Empty, "If disabled, CodeBucket will prompt you for your password when you switch to this account.") {
                    new TrueFalseElement("Remember Credentials", !currentAccount.DontRemember, (e) => { 
                        currentAccount.DontRemember = !e.Value; 
                        currentAccount.Update();
                    })
            });

            root.Add(new Section(string.Empty, "If enabled, your teams will be shown in the CodeBucket slideout menu.") {
                new TrueFalseElement("Show Teams in Menu", !currentAccount.DontShowTeamEvents, (e) => { 
                    currentAccount.DontShowTeamEvents = !e.Value; 
                    currentAccount.Update();
                })
            });

			//Assign the root
			Root = root;
        }



        private void OpenUserVoice()
        {
            var config = UserVoice.UVConfig.Create("http://codebucket.uservoice.com", "pnuDmPENErDiDpXrms1DTg", "iDboMdCIwe2E5hJFa8hy9K9I5wZqnjKCE0RPHLhZIk");
            UserVoice.UserVoice.PresentUserVoiceInterface(this, config);
        }

    }
}


