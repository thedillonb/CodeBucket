using System;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Users;
using System.Threading.Tasks;
using System.Linq;
using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Issues;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Groups;
using Splat;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Client;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.ViewModels.App
{
    public class MenuViewModel : BaseViewModel, ILoadableViewModel, ISupportsActivation
    {
        private readonly IApplicationService _applicationService;

        public ReactiveCommand<Unit, Unit> GoToDefaultTopView { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        public ReactiveList<GroupItemViewModel> Groups { get; } = new ReactiveList<GroupItemViewModel>();

        public IReadOnlyReactiveList<TeamItemViewModel> Teams { get; }

        public IReadOnlyReactiveList<TeamItemViewModel> TeamEvents { get; }

        public IReadOnlyReactiveList<PinnedRepositoryItemViewModel> PinnedRepositories { get; }

        public bool ExpandTeamsAndGroups => _applicationService.Account.ExpandTeamsAndGroups;

        public bool ShowTeamEvents => _applicationService.Account.ShowTeamEvents;

        public string Username { get; }

        public Avatar Avatar { get; }

        public MenuViewModel(
            IApplicationService applicationService = null,
            IAccountsService accountsService = null)
        {
            _applicationService = applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();

            var account = applicationService.Account;
            Avatar = new Avatar(account.AvatarUrl);
            Username = account.Username;
            var username = Username;

            Title = username;

            var repos = new ReactiveList<PinnedRepository>();
            PinnedRepositories = repos.CreateDerivedCollection(x =>
            {
                var vm = new PinnedRepositoryItemViewModel(x.Name, new Avatar(x.ImageUri));
                vm.DeleteCommand
                  .Do(_ => account.PinnedRepositories.RemoveAll(y => y.Id == x.Id))
                  .Subscribe(_ => repos.Remove(x));
                vm.GoToCommand
                  .Select(_ => new RepositoryViewModel(x.Owner, x.Slug))
                  .Subscribe(NavigateTo);
                return vm;
            });

            RefreshCommand = ReactiveCommand.CreateFromTask(_ =>
            {
                repos.Reset(applicationService.Account.PinnedRepositories);
                return Task.FromResult(Unit.Default);
            });

            var teams = new ReactiveList<User>();
            Teams = teams.CreateDerivedCollection(x =>
            {
                var viewModel = new TeamItemViewModel(x.Username);
                viewModel.GoToCommand
                         .Select(_ => new TeamViewModel(x))
                         .Subscribe(NavigateTo);
                return viewModel;
            });

            TeamEvents = teams.CreateDerivedCollection(x =>
            {
                var viewModel = new TeamItemViewModel(x.Username);
                viewModel.GoToCommand
                         .Select(_ => new UserEventsViewModel(x.Username))
                         .Subscribe(NavigateTo);
                return viewModel;
            });

            LoadCommand = ReactiveCommand.CreateFromTask(t =>
            {
                applicationService.Client
                                  .AllItems(x => x.Teams.GetAll())
                                  .ToBackground(teams.Reset);

                applicationService.Client.Groups
                                  .GetGroups(username)
                                  .ToBackground(groups => Groups.Reset(groups.Select(ToViewModel)));

                return Task.FromResult(Unit.Default);
            });

            GoToProfileCommand
                .Select(_ => new UserViewModel(username))
                .Subscribe(NavigateTo);

            GoToMyEvents
                .Select(_ => new UserEventsViewModel(username))
                .Subscribe(NavigateTo);

            GoToStarredRepositoriesCommand
                .Select(_ => new RepositoriesStarredViewModel())
                .Subscribe(NavigateTo);

            GoToOwnedRepositoriesCommand
                .Select(_ => new UserRepositoriesViewModel(username))
                .Subscribe(NavigateTo);

            GoToSharedRepositoriesCommand
                .Select(_ => new RepositoriesSharedViewModel())
                .Subscribe(NavigateTo);

            GoToTeamsCommand
                .Select(_ => new TeamsViewModel())
                .Subscribe(NavigateTo);

            GoToSettingsCommand
                .Select(_ => new SettingsViewModel())
                .Subscribe(NavigateTo);

            GoToFeedbackCommand
                .Select(_ => new IssuesViewModel("thedillonb", "codebucket"))
                .Subscribe(NavigateTo);

            GoToGroupsCommand
                .Select(_ => new GroupsViewModel(username))
                .Subscribe(NavigateTo);

            GoToExploreRepositoriesCommand
                .Select(_ => new RepositoriesExploreViewModel())
                .Subscribe(NavigateTo);

            GoToDefaultTopView.Subscribe(_ =>
            {
                var startupViewName = applicationService.Account.DefaultStartupView;
                if (!string.IsNullOrEmpty(startupViewName))
                {
                    var props = from p in GetType().GetProperties()
                                let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
                                where attr.Length == 1
                                select new { Property = p, Attribute = attr[0] as PotentialStartupViewAttribute };


                    var match = props.FirstOrDefault(x => string.Equals(startupViewName, x.Attribute.Name));
                    var cmd = match?.Property.GetValue(this) as ReactiveCommand<Unit, Unit>;
                    if (cmd != null)
                    {
                        cmd.ExecuteNow();
                        return;
                    }
                }

                //Oh no... Look for the last resort DefaultStartupViewAttribute
                var deprop = (from p in GetType().GetProperties()
                              let attr = p.GetCustomAttributes(typeof(DefaultStartupViewAttribute), true)
                              where attr.Length == 1
                              select new { Property = p, Attribute = attr[0] as DefaultStartupViewAttribute }).FirstOrDefault();

                //That shouldn't happen...
                var bCmd = deprop?.Property.GetValue(this) as ReactiveCommand<Unit, Unit>;
                if (bCmd != null)
                    bCmd.ExecuteNow();
            });
        }

        private GroupItemViewModel ToViewModel(Client.V1.Group group)
        {
            var viewModel = new GroupItemViewModel(group.Name);
            viewModel.GoToCommand
                     .Select(_ => new GroupViewModel(group))
                     .Subscribe(NavigateTo);
            return viewModel;
        }

        [PotentialStartupView("Profile")]
        public ReactiveCommand<Unit, Unit> GoToProfileCommand { get; } = ReactiveCommandFactory.Empty();

        [DefaultStartupView]
        [PotentialStartupView("My Events")]
        public ReactiveCommand<Unit, Unit> GoToMyEvents { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("Starred Repositories")]
        public ReactiveCommand<Unit, Unit> GoToStarredRepositoriesCommand { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("My Repositories")]
        public ReactiveCommand<Unit, Unit> GoToOwnedRepositoriesCommand { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("Shared Repositories")]
        public ReactiveCommand<Unit, Unit> GoToSharedRepositoriesCommand { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("Explore Repositories")]
        public ReactiveCommand<Unit, Unit> GoToExploreRepositoriesCommand { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("Organizations")]
        public ReactiveCommand<Unit, Unit> GoToGroupsCommand { get; } = ReactiveCommandFactory.Empty();

        [PotentialStartupView("Teams")]
        public ReactiveCommand<Unit, Unit> GoToTeamsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToSettingsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToFeedbackCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();
    }
}
