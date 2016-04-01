using System;
using BitbucketSharp.Models;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.ViewModels.Events;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamViewModel : LoadableViewModel
    {
        private UserModel _userModel;

        public string Name
        {
            get;
            private set;
        }

        public UserModel User
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

        protected override Task Load(bool forceCacheInvalidation)
        {
            return this.RequestModel(() => this.GetApplication().Client.Users[Name].GetInfo(forceCacheInvalidation), response => User = response.User);
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

