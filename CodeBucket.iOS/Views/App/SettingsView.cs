using Cirrious.CrossCore;
using CodeBucket.Core.Services;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.Elements;

namespace CodeBucket.Views.App
{
    public class SettingsView : PrettyDialogViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            var application = Mvx.Resolve<IApplicationService>();
            base.ViewWillAppear(animated);
            Title = application.Account.Username;
            HeaderView.SetImage(null, Images.Avatar);
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
			root.Add(new Section("Apperance") { showOrganizationsInEvents, showOrganizations, repoDescriptions, startupView });
			Root = root;

		}
    }
}


