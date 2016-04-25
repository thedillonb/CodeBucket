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
using BitbucketSharp;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.Teams;
using MvvmCross.Platform;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Issues;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Groups;
using BitbucketSharp.Models.V2;

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

        public ReactiveUI.ReactiveList<GroupItemViewModel> Groups { get; } = new ReactiveUI.ReactiveList<GroupItemViewModel>();

        public ReactiveUI.ReactiveList<TeamItemViewModel> Teams { get; } = new ReactiveUI.ReactiveList<TeamItemViewModel>();

		public BitbucketAccount Account
        {
            get { return _application.Account; }
        }
		
        public MenuViewModel(IApplicationService application)
        {
            _application = application;

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(t =>
            {
                application.Client
                           .AllItems(x => x.Teams.GetTeams(BitbucketSharp.Controllers.TeamRole.Member))
                           .ToBackground(x => Teams.Reset(x.Select(ToViewModel)));

                application.Client.Groups
                           .GetGroups(Account.Username)
                           .ToBackground(groups => Groups.Reset(groups.Select(ToViewModel)));

                return Task.FromResult(0);
            });
        }

        private TeamItemViewModel ToViewModel(Team team)
        {
            var viewModel = new TeamItemViewModel(team.Username);
            viewModel.GoToCommand
                     .Select(_ => new TeamViewModel.NavObject { Name = team.Username })
                     .Subscribe(y => ShowMenuViewModel<TeamViewModel>(y));
            return viewModel;
        }

        private GroupItemViewModel ToViewModel(GroupModel group)
        {
            var viewModel = new GroupItemViewModel(group.Name);
            viewModel.GoToCommand
                     .Select(_ => new GroupViewModel.NavObject { Slug = group.Slug, Owner = group.Owner.Username, Name = group.Name })
                     .Subscribe(y => ShowMenuViewModel<GroupViewModel>(y));
            return viewModel;
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
