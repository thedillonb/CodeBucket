using Cirrious.CrossCore;
using CodeBucket.Core.Services;
using MonoTouch.Dialog;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.Elements;

namespace CodeBucket.Views.App
{
	public class SettingsView : ViewModelDrivenDialogViewController
    {
        public SettingsView()
        {
            Title = "Settings";
        }

        public override void ViewWillAppear(bool animated)
        {
			CreateTable();
            base.ViewWillAppear(animated);
        }

		private void CreateTable()
		{
			var application = Mvx.Resolve<IApplicationService>();
			var vm = (SettingsViewModel)ViewModel;
			var currentAccount = application.Account;

			var saveCredentials = new TrueFalseElement("Save Credentials", !currentAccount.DontRemember, e =>
				{ 
					currentAccount.DontRemember = !e.Value;
					application.Accounts.Update(currentAccount);
				});

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

			//var pushNotifications = new TrueFalseElement("Push Notifications", vm.PushNotificationsEnabled, e => vm.PushNotificationsEnabled = e.Value);

			var totalCacheSizeMB = vm.CacheSize.ToString("0.##");
			var deleteCache = new StyledStringElement("Delete Cache", string.Format("{0} MB", totalCacheSizeMB), UIKit.UITableViewCellStyle.Value1);
			deleteCache.Tapped += () =>
			{ 
				vm.DeleteAllCacheCommand.Execute(null);
				deleteCache.Value = string.Format("{0} MB", 0);
				ReloadData();
			};

			//Assign the root
			var root = new RootElement(Title);
			root.Add(new Section("Account") { saveCredentials /*			, pushNotifications */ });
			root.Add(new Section("Apperance") { showOrganizationsInEvents, showOrganizations, repoDescriptions, startupView });
			root.Add(new Section ("Internal") { deleteCache });
			Root = root;

		}
    }
}


