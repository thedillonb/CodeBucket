using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;

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

        public ICommand GoToFollowersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToFollowingCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowingsViewModel>(new UserFollowingsViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToEventsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserEventsViewModel>(new UserEventsViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToGroupsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<GroupsViewModel>(new GroupsViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToRepositoriesCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserRepositoriesViewModel>(new UserRepositoriesViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToMembersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<TeamMembersViewModel>(new TeamMembersViewModel.NavObject { Name = Name })); }
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        public TeamViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Team = await applicationService.Client.Teams.GetTeam(Name);
            });
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

