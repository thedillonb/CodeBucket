using System;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.User
{
    public class UserViewModel : BaseViewModel, ILoadableViewModel
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            private set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private bool _showGroups;
        public bool ShouldShowGroups
        {
            get { return _showGroups; }
            private set { this.RaiseAndSetIfChanged(ref _showGroups, value); }
        }

        private BitbucketSharp.Models.V2.User _user;
        public BitbucketSharp.Models.V2.User User
        {
            get { return _user; }
            private set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public ReactiveUI.IReactiveCommand<object> GoToFollowersCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToFollowingCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToGroupsCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToRepositoriesCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public UserViewModel(IApplicationService applicationService)
        {
            GoToFollowersCommand
                .Select(_ => new UserFollowersViewModel.NavObject { Username = Username })
                .Subscribe(x => ShowViewModel<UserFollowersViewModel>(x));

            GoToFollowingCommand
                .Select(_ => new UserFollowingsViewModel.NavObject { Username = Username })
                .Subscribe(x => ShowViewModel<UserFollowingsViewModel>(x));

            GoToEventsCommand
                .Select(_ => new UserEventsViewModel.NavObject { Username = Username })
                .Subscribe(x => ShowViewModel<UserEventsViewModel>(x));

            GoToGroupsCommand
                .Select(_ => new GroupsViewModel.NavObject { Username = Username })
                .Subscribe(x => ShowViewModel<GroupsViewModel>(x));

            GoToRepositoriesCommand
                .Select(_ => new UserRepositoriesViewModel.NavObject { Username = Username })
                .Subscribe(x => ShowViewModel<UserRepositoriesViewModel>(x));

            this.Bind(x => x.Username, true)
                .Select(x => string.Equals(x, applicationService.Account.Username, StringComparison.OrdinalIgnoreCase))
                .Subscribe(x => ShouldShowGroups = x);

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async t => 
            {
                if (!string.Equals(applicationService.Account.Username, Username, StringComparison.OrdinalIgnoreCase))
                {
                    applicationService.Client.Groups.GetGroups(Username)
                                  .ToObservable()
                                  .Select(_ => true)
                                  .Catch(Observable.Return(false))
                                  .Subscribe(x => ShouldShowGroups = x);
                }

                User = await applicationService.Client.Users.GetUser(Username);
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

