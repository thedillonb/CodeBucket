using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using UIKit;
using Foundation;
using System;
using CodeBucket.DialogElements;
using System.Collections.Generic;
using ReactiveUI;

namespace CodeBucket.ViewController.Application
{
    public class SettingsViewController : PrettyDialogViewController<SettingsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RefreshHeaderView(subtext: ViewModel.Account.Username, image: Images.Avatar);

            var currentAccount = ViewModel.Account;
            var showOrganizationsInEvents = new BooleanElement("Show Teams under Events", currentAccount.ShowTeamEvents);
            var showOrganizations = new BooleanElement("List Teams & Groups in Menu", currentAccount.ExpandTeamsAndGroups);
            var repoDescriptions = new BooleanElement("Show Repo Descriptions", currentAccount.RepositoryDescriptionInList);
            var startupView = new ButtonElement("Startup View", ViewModel.DefaultStartupViewName);
            var sourceCommand = new ButtonElement("Source Code");
            var twitter = new ButtonElement("Follow On Twitter");
            var rate = new ButtonElement("Rate This App");

            //Assign the root
            var root = new List<Section>();
            root.Add(new Section());
            root.Add(new Section { showOrganizationsInEvents, showOrganizations, repoDescriptions, startupView });
            root.Add(new Section(string.Empty, "Thank you for downloading. Enjoy!")
            {
                sourceCommand, twitter, rate,
                new StringElement("App Version", NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString())
            });
            Root.Reset(root);

            OnActivation(disposable =>
            {
                showOrganizationsInEvents
                    .Changed
                    .Subscribe(e => ViewModel.SetAccount(x => x.ShowTeamEvents = e))
                    .AddTo(disposable);

                showOrganizations
                    .Changed
                    .Subscribe(e => ViewModel.SetAccount(x => x.ExpandTeamsAndGroups = e))
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.DefaultStartupViewName)
                    .Subscribe(x => startupView.Value = x)
                    .AddTo(disposable);

                startupView
                    .Clicked
                    .Subscribe(_ => NavigationController.PushViewController(new DefaultStartupViewController(), true))
                    .AddTo(disposable);

                repoDescriptions
                    .Changed
                    .Subscribe(e => ViewModel.SetAccount(x => x.RepositoryDescriptionInList = e))
                    .AddTo(disposable);

                sourceCommand
                    .Clicked
                    .Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://bitbucket.org/thedillonb/codebucket")))
                    .AddTo(disposable);

                twitter
                    .Clicked
                    .Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp")))
                    .AddTo(disposable);

                rate
                    .Clicked
                    .Subscribe(_ => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8")))
                    .AddTo(disposable);
            });
        }
    }
}


