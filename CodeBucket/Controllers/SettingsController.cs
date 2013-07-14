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
		private Section _accountOptionsSection;

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


