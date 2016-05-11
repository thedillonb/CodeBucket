using System;
using CodeBucket.Core.ViewModels.Events;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Splat;
using ReactiveUI;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserViewModel : BaseViewModel, ILoadableViewModel
    {
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

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> GoToFollowersCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToFollowingCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToGroupsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToRepositoriesCommand { get; } = ReactiveCommand.Create();

        public UserViewModel(string username, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            GoToFollowersCommand.Subscribe(_ => NavigateTo(new UserFollowersViewModel(username)));
            GoToFollowingCommand.Subscribe(_ => NavigateTo(new UserFollowingsViewModel(username)));
            GoToEventsCommand.Subscribe(_ => NavigateTo(new UserEventsViewModel(username)));
            GoToGroupsCommand.Subscribe(_ => NavigateTo(new GroupsViewModel(username)));
            GoToRepositoriesCommand.Subscribe(_ => NavigateTo(new UserRepositoriesViewModel(username)));

            ShouldShowGroups = string.Equals(username, applicationService.Account.Username, StringComparison.OrdinalIgnoreCase);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => 
            {
                if (!string.Equals(applicationService.Account.Username, username, StringComparison.OrdinalIgnoreCase))
                {
                    applicationService.Client.Groups.GetGroups(username)
                                  .ToObservable()
                                  .Select(_ => true)
                                  .Catch(Observable.Return(false))
                                  .Subscribe(x => ShouldShowGroups = x);
                }

                User = await applicationService.Client.Users.GetUser(username);
            });
        }
    }
}

