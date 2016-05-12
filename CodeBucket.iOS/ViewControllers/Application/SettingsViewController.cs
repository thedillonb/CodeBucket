using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using UIKit;
using Foundation;
using System;
using CodeBucket.DialogElements;
using System.Collections.Generic;

namespace CodeBucket.ViewController.Application
{
    public class SettingsViewController : PrettyDialogViewController<SettingsViewModel>
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = ViewModel.Title;
            HeaderView.SetImage(null, Images.Avatar);
            CreateTable();
        }

		private void CreateTable()
		{
            var currentAccount = ViewModel.Account;

            var showOrganizationsInEvents = new BooleanElement("Show Teams under Events", currentAccount.ShowTeamEvents);
            showOrganizationsInEvents.Changed.Subscribe(e => ViewModel.SetAccount(x => x.ShowTeamEvents = e));

            var showOrganizations = new BooleanElement("List Teams & Groups in Menu", currentAccount.ExpandTeamsAndGroups);
            showOrganizations.Changed.Subscribe(e => ViewModel.SetAccount(x => x.ExpandTeamsAndGroups = e));

            var repoDescriptions = new BooleanElement("Show Repo Descriptions", currentAccount.RepositoryDescriptionInList);
            repoDescriptions.Changed.Subscribe(e => ViewModel.SetAccount(x => x.RepositoryDescriptionInList = e));

            var startupView = new ButtonElement("Startup View", ViewModel.DefaultStartupViewName);
            startupView.Clicked.Subscribe(_ =>
                NavigationController.PushViewController(new DefaultStartupViewController(), true));

            var sourceCommand = new ButtonElement("Source Code");
            sourceCommand.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://github.com/thedillonb/CodeBucket")));

            var twitter = new StringElement("Follow On Twitter");
            twitter.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp")));

            var rate = new StringElement("Rate This App");
            rate.Clicked.Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8")));

			//Assign the root
            var root = new List<Section>();
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


