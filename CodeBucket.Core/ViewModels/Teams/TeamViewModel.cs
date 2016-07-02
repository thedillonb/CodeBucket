using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using System.Threading.Tasks;
using CodeBucket.Client.Models;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamViewModel : LoadableViewModel
    {
        private User _userModel;

        public string Name
        {
            get;
            private set;
        }

        public User User
        {
            get { return _userModel; }
            private set { _userModel = value; RaisePropertyChanged(() => User); }
        }

        public ICommand GoToFollowersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Name })); }
        }

        public ICommand GoToFollowingCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowingsViewModel>(new UserFollowingsViewModel.NavObject { Name = Name })); }
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

        protected override async Task Load()
        {
            User = await this.GetApplication().Client.Teams.Get(Name);
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

