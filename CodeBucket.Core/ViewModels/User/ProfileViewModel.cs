using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.User
{
    public class ProfileViewModel : BaseViewModel, ILoadableViewModel
    {
        public string Username { get; private set; }

        private BitbucketSharp.Models.V2.User _user;
        public BitbucketSharp.Models.V2.User User
        {
            get { return _user; }
            private set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public ICommand GoToFollowersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowersViewModel>(new UserFollowersViewModel.NavObject { Username = Username })); }
        }

        public ICommand GoToFollowingCommand
        {
            get { return new MvxCommand(() => ShowViewModel<UserFollowingsViewModel>(new UserFollowingsViewModel.NavObject { Username = Username })); }
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

        public ProfileViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async t =>
            {
                User = await applicationService.Client.Users.GetUser();
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

