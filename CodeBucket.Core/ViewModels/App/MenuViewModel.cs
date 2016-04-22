using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Users;
using System.Threading.Tasks;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.Teams;
using MvvmCross.Platform;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Issues;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.App
{
    public class MenuViewModel : BaseViewModel, ILoadableViewModel
    {
        private static readonly IDictionary<string, string> Presentation = new Dictionary<string, string> {{PresentationValues.SlideoutRootPresentation, string.Empty}};  

        private static IAccountsService Accounts
        {
            get { return Mvx.Resolve<IAccountsService>(); }
        }

        public void Init()
        {
            GoToDefaultTopView.Execute(null);
        }

        public ICommand GoToDefaultTopView
        {
            get
            {
                var startupViewName = Accounts.ActiveAccount.DefaultStartupView;
                if (!string.IsNullOrEmpty(startupViewName))
                {
                    var props = from p in GetType().GetProperties()
                        let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
                            where attr.Length == 1
                        select new { Property = p, Attribute = attr[0] as PotentialStartupViewAttribute};

                    foreach (var p in props)
                    {
                        if (string.Equals(startupViewName, p.Attribute.Name))
                            return p.Property.GetValue(this) as ICommand;
                    }
                }

                //Oh no... Look for the last resort DefaultStartupViewAttribute
                var deprop = (from p in GetType().GetProperties()
                    let attr = p.GetCustomAttributes(typeof(DefaultStartupViewAttribute), true)
                    where attr.Length == 1
                    select new { Property = p, Attribute = attr[0] as DefaultStartupViewAttribute }).FirstOrDefault();

                //That shouldn't happen...
                if (deprop == null)
                    return null;
                var val = deprop.Property.GetValue(this);
                return val as ICommand;
            }
        }

        public ICommand DeletePinnedRepositoryCommand
        {
            get 
            {
                return new MvxCommand<CodeFramework.Core.Data.PinnedRepository>(x => Accounts.ActiveAccount.PinnnedRepositories.RemovePinnedRepository(x.Id), x => x != null);
            }
        }

        protected bool ShowMenuViewModel<T>(object data) where T : IMvxViewModel
        {
            return this.ShowViewModel<T>(data, new MvxBundle(Presentation));
        }

        public IEnumerable<CodeFramework.Core.Data.PinnedRepository> PinnedRepositories
        {
            get { return Accounts.ActiveAccount.PinnnedRepositories; }
        }

        private readonly IApplicationService _application;

		private List<GroupModel> _groups;
		public List<GroupModel> Groups
		{
			get { return _groups; }
            set { this.RaiseAndSetIfChanged(ref _groups, value); }
		}

		private List<string> _teams;
		public List<string> Teams
		{
			get { return _teams; }
            set { this.RaiseAndSetIfChanged(ref _teams, value); }
		}

		public BitbucketAccount Account
        {
            get { return _application.Account; }
        }
		
        public MenuViewModel(IApplicationService application)
        {
            _application = application;

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(_ =>
            {
                application.Client.Users.GetPrivileges()
                    .ToObservable()
                    .Where(x => x.Teams != null)
                    .Subscribe(x =>
                    {
                        var teams = x.Teams.Keys.ToList();
                        teams.Remove(Account.Username);
                        Teams = teams;
                    });

                application.Client.Groups.GetGroups(Account.Username)
                    .ToObservable()
                    .Subscribe(x => Groups = x);

                return Task.FromResult(0);
            });
        }

		[PotentialStartupViewAttribute("Profile")]
        public ICommand GoToProfileCommand
        {
            get { return new MvxCommand(() => ShowMenuViewModel<UserViewModel>(new UserViewModel.NavObject { Username = _application.Account.Username })); }
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
            get { return new MvxCommand(() => ShowMenuViewModel<UserRepositoriesViewModel>(new UserRepositoriesViewModel.NavObject { Username = Account.Username })); }
		}

        [PotentialStartupViewAttribute("Shared Repositories")]
        public ICommand GoToSharedRepositoriesCommand
        {
            get { return new MvxCommand(() => ShowMenuViewModel<RepositoriesSharedViewModel>(null)); }
        }

		[PotentialStartupViewAttribute("Explore Repositories")]
        public ReactiveUI.IReactiveCommand<object> GoToExploreRepositoriesCommand { get; } = ReactiveUI.ReactiveCommand.Create();

		public ICommand GoToTeamEventsCommand
		{
			get { return new MvxCommand<string>(x => ShowMenuViewModel<Events.UserEventsViewModel>(new Events.UserEventsViewModel.NavObject { Username = x }));}
		}

		public ICommand GoToGroupCommand
		{
			get { return new MvxCommand<GroupModel>(x => ShowMenuViewModel<Groups.GroupViewModel>(new Groups.GroupViewModel.NavObject { Owner = x.Owner.Username, Name = x.Name }));}
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

        public ICommand GoToFeedbackCommand
        {
            get { return new MvxCommand(() => ShowMenuViewModel<IssuesViewModel>(new IssuesViewModel.NavObject { Repository = "CodeBucket", Username = "thedillonb" }));}
        }


		public ICommand GoToRepositoryCommand
		{
			get { return new MvxCommand<RepositoryIdentifier>(x => ShowMenuViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, RepositorySlug = x.Name }));}
		}

        public ReactiveUI.IReactiveCommand LoadCommand { get; }
    }
}
