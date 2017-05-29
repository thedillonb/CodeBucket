using System;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive;
using Splat;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamViewModel : BaseViewModel, ILoadableViewModel
    {
        private User _team;
        public User Team
        {
            get { return _team; }
            private set { this.RaiseAndSetIfChanged(ref _team, value); }
        }

        private readonly ObservableAsPropertyHelper<string> _displayName;
        public string DisplayName => _displayName.Value;

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToFollowersCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToFollowingCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToEventsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToGroupsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToRepositoriesCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToMembersCommand { get; } = ReactiveCommandFactory.Empty();

        public TeamViewModel(
            User team,
            IApplicationService applicationService = null)
            : this(team.Username, applicationService)
        {
            Team = team;
        }

        public TeamViewModel(
            string name, 
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = name;

            GoToFollowersCommand.Subscribe(_ => NavigateTo(new TeamFollowersViewModel(name)));
            GoToFollowingCommand.Subscribe(_ => NavigateTo(new TeamFollowingsViewModel(name)));
            GoToMembersCommand.Subscribe(_ => NavigateTo(new TeamMembersViewModel(name)));
            GoToGroupsCommand.Subscribe(_ => NavigateTo(new GroupsViewModel(name)));
            GoToEventsCommand.Subscribe(_ => NavigateTo(new UserEventsViewModel(name)));
            GoToRepositoriesCommand.Subscribe(_ => NavigateTo(new UserRepositoriesViewModel(name)));

            this.WhenAnyValue(x => x.Team.DisplayName)
                .Select(x => string.Equals(x, name) ? null : x)
                .ToProperty(this, x => x.DisplayName, out _displayName);

            LoadCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                Team = await applicationService.Client.Teams.Get(name);
            });
        }
    }
}

