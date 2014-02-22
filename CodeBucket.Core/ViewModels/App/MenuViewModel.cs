using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.User;
using CodeFramework.Core.Utils;
using CodeFramework.Core.ViewModels.App;
using System.Threading.Tasks;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.Teams;

namespace CodeBucket.Core.ViewModels.App
{
	public class MenuViewModel : BaseMenuViewModel
    {
        private readonly IApplicationService _application;

		private List<GroupModel> _groups;
		public List<GroupModel> Groups
		{
			get { return _groups; }
			set
			{
				_groups = value;
				RaisePropertyChanged(() => Groups);
			}
		}

		private List<string> _teams;
		public List<string> Teams
		{
			get { return _teams; }
			set
			{
				_teams = value;
				RaisePropertyChanged(() => Teams);
			}
		}

		public BitbucketAccount Account
        {
            get { return _application.Account; }
        }
		
        public MenuViewModel(IApplicationService application)
        {
            _application = application;
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

        [PotentialStartupViewAttribute("My Repositories")]
		public ICommand GoToOwnedRepositoriesCommand
		{
            get { return new MvxCommand(() => ShowMenuViewModel<MyRepositoriesViewModel>(null)); }
		}

		[PotentialStartupViewAttribute("Explore Repositories")]
		public ICommand GoToExploreRepositoriesCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<RepositoriesExploreViewModel>(null));}
		}

		public ICommand GoToTeamEventsCommand
		{
			get { return new MvxCommand<string>(x => ShowMenuViewModel<Events.UserEventsViewModel>(new Events.UserEventsViewModel.NavObject { Username = x }));}
		}

		public ICommand GoToGroupCommand
		{
			get { return new MvxCommand<GroupModel>(x => ShowMenuViewModel<Groups.GroupViewModel>(new Groups.GroupViewModel.NavObject { Username = x.Owner.Username, GroupName = x.Name }));}
		}

		[PotentialStartupViewAttribute("Organizations")]
		public ICommand GoToGroupsCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<Groups.GroupsViewModel>(new Groups.GroupsViewModel.NavObject { Username = Account.Username }));}
		}

		[PotentialStartupViewAttribute("Teams")]
		public ICommand GoToTeamsCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<TeamsViewModel>(null)); }
		}

		public ICommand GoToTeamCommand
		{
            get { return new MvxCommand<string>(x => ShowMenuViewModel<TeamViewModel>(new TeamViewModel.NavObject { Name = x })); }
		}

	
		public ICommand GoToSettingsCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<SettingsViewModel>(null));}
		}

		public ICommand GoToAboutCommand
		{
			get { return new MvxCommand(() => ShowMenuViewModel<AboutViewModel>(null));}
		}

		public ICommand GoToRepositoryCommand
		{
			get { return new MvxCommand<RepositoryIdentifier>(x => ShowMenuViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, RepositorySlug = x.Name }));}
		}

        public ICommand LoadCommand
        {
            get { return new MvxCommand(Load);}    
        }

        private void Load()
        {
			Task.Run(() => this.GetApplication().Client.Account.GetPrivileges()).ContinueWith(x =>
			{
				if (x.Result != null && x.Result.Teams != null)
				{
					var teams = x.Result.Teams.Keys.ToList();
					teams.Remove(Account.Username);
					Teams = teams;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext()).FireAndForget();

			Task.Run(() => this.GetApplication().Client.Account.Groups.GetGroups()).ContinueWith(x => Groups = x.Result, TaskScheduler.FromCurrentSynchronizationContext()).FireAndForget();
        }
    }
}
