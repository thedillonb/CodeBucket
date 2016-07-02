using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Client.Models;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;

namespace CodeBucket.Core.ViewModels.Users
{
    public class ProfileViewModel : LoadableViewModel
    {
        private UserModel _userModel;

        public string Username
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
            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToFollowingCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowingsViewModel>(new UserFollowingsViewModel.NavObject { Name = Username })); }
        }

        public ICommand GoToEventsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserEventsViewModel>(new UserEventsViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToGroupsCommand
        {
			get { return new MvxCommand(() => ShowViewModel<GroupsViewModel>(new GroupsViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToRepositoriesCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserRepositoriesViewModel>(new UserRepositoriesViewModel.NavObject { Username = Username })); }
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        protected override async Task Load()
        {
            var response = await this.GetApplication().Client.Users.GetUser(Username);
            User = response.User;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

