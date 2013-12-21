using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.User;
using System.Linq;
using CodeFramework.Core.Utils;
using CodeFramework.Core.ViewModels.App;
using CodeBucket.Core.Messages;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.ViewModels.App
{
	public class MenuViewModel : BaseMenuViewModel
    {
        private readonly IApplicationService _application;
		private int _notifications;
		private List<string> _organizations;
		private readonly MvxSubscriptionToken _notificationCountToken;

		public int Notifications
        {
            get { return _notifications; }
            set { _notifications = value; RaisePropertyChanged(() => Notifications); }
        }

		public BitbucketAccount Account
        {
            get { return _application.Account; }
        }
		
        public MenuViewModel(IApplicationService application)
        {
            _application = application;
			_notificationCountToken = Messenger.SubscribeOnMainThread<NotificationCountMessage>(OnNotificationCountMessage);
        }

		private void OnNotificationCountMessage(NotificationCountMessage msg)
		{
			Notifications = msg.Count;
		}

        public ICommand GoToAccountsCommand
        {
            get { return new MvxCommand(() => this.ShowViewModel<AccountsViewModel>()); }
        }

		[PotentialStartupViewAttribute("Profile")]
        public ICommand GoToProfileCommand
        {
            get { return new MvxCommand(() => ShowMenuViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = _application.Account.Username })); }
        }

		[DefaultStartupViewAttribute]
		[PotentialStartupViewAttribute("My Events")]
        public ICommand GoToMyEvents
        {
            get { return new MvxCommand(() => ShowMenuViewModel<UserEventsViewModel>(new UserEventsViewModel.NavObject { Username = Account.Username })); }
        }

		[PotentialStartupViewAttribute("Starred Repositories")]
        public ICommand GoToStarredRepositoriesCommand
        {
			get { return new MvxCommand(() => ShowMenuViewModel<RepositoriesStarredViewModel>(null));}
        }

		[PotentialStartupViewAttribute("Owned Repositories")]
		public ICommand GoToOwnedRepositoriesCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<UserRepositoriesViewModel>(new UserRepositoriesViewModel.NavObject { Username = Account.Username }));}
		}

		[PotentialStartupViewAttribute("Explore Repositories")]
		public ICommand GoToExploreRepositoriesCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<RepositoriesExploreViewModel>(null));}
		}

		public ICommand GoToOrganizationEventsCommand
		{
			get { return new MvxCommand<string>(x => ShowMenuViewModel<Events.UserEventsViewModel>(new Events.UserEventsViewModel.NavObject { Username = x }));}
		}

		public ICommand GoToGroupCommand
		{
			get { return new MvxCommand<string>(x => ShowMenuViewModel<Groups.GroupsViewModel>(new Groups.GroupsViewModel.NavObject { Username = x }));}
		}

		[PotentialStartupViewAttribute("Organizations")]
		public ICommand GoToGroupsCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<Groups.GroupsViewModel>(new Groups.GroupsViewModel.NavObject { Username = Account.Username }));}
		}
	
		public ICommand GoToSettingsCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<SettingsViewModel>(null));}
		}

		public ICommand GoToRepositoryCommand
		{
			get { return new MvxCommand<Utils.RepositoryIdentifier>(x => ShowMenuViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, Repository = x.Name }));}
		}

        public ICommand LoadCommand
        {
            get { return new MvxCommand(Load);}    
        }

        private async void Load()
        {
//			var t1 = this.GetApplication().Client.ExecuteAsync(this.GetApplication().Client.Notifications.GetAll()).ContinueWith(x =>
//			{
//				Notifications = x.Result.Data.Count;
//			}, TaskContinuationOptions.OnlyOnRanToCompletion);

//			var t2 = this.GetApplication().Client.ExecuteAsync(this.GetApplication().Client.AuthenticatedUser.GetOrganizations()).ContinueWith(x =>
//			{
//				Organizations = x.Result.Data.Select(y => y.Login).ToList();
//			},TaskContinuationOptions.OnlyOnRanToCompletion);
//
//			await Task.WhenAll(t1, t2);
        }
    }
}
