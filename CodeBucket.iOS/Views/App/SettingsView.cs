using Cirrious.CrossCore;
using CodeBucket.Core.Services;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.Elements;
using UIKit;
using Foundation;
using System;

namespace CodeBucket.Views.App
{
    public class SettingsView : PrettyDialogViewController
    {
        public SettingsView()
        {
            Title = "Settings";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var application = Mvx.Resolve<IApplicationService>();
            HeaderView.Text = application.Account.Username;
            HeaderView.SubText = Title;
            HeaderView.SetImage(null, Images.Avatar);
            RefreshHeaderView();

            CreateTable();
        }

		private void CreateTable()
		{
			var application = Mvx.Resolve<IApplicationService>();
			var vm = (SettingsViewModel)ViewModel;
			var currentAccount = application.Account;

			var showOrganizationsInEvents = new TrueFalseElement("Show Teams under Events", currentAccount.ShowTeamEvents, e =>
			{ 
				currentAccount.ShowTeamEvents = e.Value;
				application.Accounts.Update(currentAccount);
			});

			var showOrganizations = new TrueFalseElement("List Teams & Groups in Menu", currentAccount.ExpandTeamsAndGroups, e =>
			{ 
				currentAccount.ExpandTeamsAndGroups = e.Value;
				application.Accounts.Update(currentAccount);
			});

			var repoDescriptions = new TrueFalseElement("Show Repo Descriptions", currentAccount.RepositoryDescriptionInList, e =>
			{ 
				currentAccount.RepositoryDescriptionInList = e.Value;
				application.Accounts.Update(currentAccount);
			});

			var startupView = new StyledStringElement("Startup View", vm.DefaultStartupViewName, UIKit.UITableViewCellStyle.Value1)
			{ 
				Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator,
			};
			startupView.Tapped += () => vm.GoToDefaultStartupViewCommand.Execute(null);

			//Assign the root
			var root = new RootElement(Title);
            root.Add(new Section());
            root.Add(new Section { showOrganizationsInEvents, showOrganizations, repoDescriptions, startupView });
            root.Add(new Section { new StyledStringElement("Source Code", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://github.com/thedillonb/CodeBucket"))) });
            root.Add(new Section(String.Empty, "Thank you for downloading. Enjoy!")
            {
                new StyledStringElement("Follow On Twitter", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp"))),
                new StyledStringElement("Rate This App", () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8"))),
                new StyledStringElement("App Version", NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString())
            });

            root.UnevenRows = true;
			Root = root;

		}
    }
}


