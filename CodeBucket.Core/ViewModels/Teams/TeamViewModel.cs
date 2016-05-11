using System;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamViewModel : BaseViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            private set { this.RaiseAndSetIfChanged(ref _team, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> GoToFollowersCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToFollowingCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToGroupsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToRepositoriesCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToMembersCommand { get; } = ReactiveCommand.Create();

        public TeamViewModel(
            Team team,
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

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Team = await applicationService.Client.Teams.GetTeam(Name);
            });
        }
    }
}

