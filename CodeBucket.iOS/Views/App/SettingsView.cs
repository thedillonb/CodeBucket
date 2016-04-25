using MvvmCross.Platform;
using CodeBucket.Core.Services;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using UIKit;
using Foundation;
using System;
using CodeBucket.DialogElements;
using System.Collections.Generic;

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

            var showOrganizationsInEvents = new BooleanElement("Show Teams under Events", currentAccount.ShowTeamEvents);
            showOrganizationsInEvents.Changed.Subscribe(e =>
			{ 
				currentAccount.ShowTeamEvents = e;
				application.Accounts.Update(currentAccount);
			});

            var showOrganizations = new BooleanElement("List Teams & Groups in Menu", currentAccount.ExpandTeamsAndGroups);
            showOrganizations.Changed.Subscribe(x =>
            {
				currentAccount.ExpandTeamsAndGroups = x;
				application.Accounts.Update(currentAccount);
			});

            var repoDescriptions = new BooleanElement("Show Repo Descriptions", currentAccount.RepositoryDescriptionInList);
            repoDescriptions.Changed.Subscribe(e =>
			{ 
				currentAccount.RepositoryDescriptionInList = e;
				application.Accounts.Update(currentAccount);
			});

            var startupView = new ButtonElement("Startup View", vm.DefaultStartupViewName);
            startupView.Clicked.Subscribe(_ =>
                NavigationController.PushViewController(new DefaultStartupView(), true));

            var sourceCommand = new ButtonElement("Source Code");
            sourceCommand.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://github.com/thedillonb/CodeBucket")));

            var twitter = new StringElement("Follow On Twitter");
            twitter.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp")));

            var rate = new StringElement("Rate This App");
            rate.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8")));

			//Assign the root
            ICollection<Section> root = new LinkedList<Section>();
            root.Add(new Section());
            root.Add(new Section { showOrganizationsInEvents, showOrganizations, repoDescriptions, startupView });
            root.Add(new Section(String.Empty, "Thank you for downloading. Enjoy!")
            {
                sourceCommand, twitter, rate,
                new StringElement("App Version", NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString())
            });
            Root.Reset(root);

		}
    }
}


