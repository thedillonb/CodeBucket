using System;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using System.Reactive.Linq;

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

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public ReactiveUI.IReactiveCommand<object> GoToFollowersCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToFollowingCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToGroupsCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToRepositoriesCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToMembersCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public TeamViewModel(IApplicationService applicationService)
        {
            GoToFollowersCommand
                .Select(_ => new TeamFollowersViewModel.NavObject { Username = Name })
                .Subscribe(x => ShowViewModel<TeamFollowersViewModel>(x));

            GoToFollowingCommand
                .Select(_ => new TeamFollowingsViewModel.NavObject { Username = Name })
                .Subscribe(x => ShowViewModel<TeamFollowingsViewModel>(x));

            GoToEventsCommand
                .Select(_ => new UserEventsViewModel.NavObject { Username = Name })
                .Subscribe(x => ShowViewModel<UserEventsViewModel>(x));

            GoToGroupsCommand
                .Select(_ => new GroupsViewModel.NavObject { Username = Name })
                .Subscribe(x => ShowViewModel<GroupsViewModel>(x));

            GoToRepositoriesCommand
                .Select(_ => new UserRepositoriesViewModel.NavObject { Username = Name })
                .Subscribe(x => ShowViewModel<UserRepositoriesViewModel>(x));

            GoToMembersCommand
                .Select(_ => new TeamMembersViewModel.NavObject { Name = Name })
                .Subscribe(x => ShowViewModel<TeamMembersViewModel>(x));

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Team = await applicationService.Client.Teams.GetTeam(Name);
            });
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

