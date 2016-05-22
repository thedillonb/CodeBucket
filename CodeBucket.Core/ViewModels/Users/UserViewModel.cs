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

        private readonly ObservableAsPropertyHelper<string> _displayName;
        public string DisplayName => _displayName.Value;

        private readonly ObservableAsPropertyHelper<bool> _isWebsiteAvailable;
        public bool IsWebsiteAvailable => _isWebsiteAvailable.Value;

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> GoToFollowersCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToFollowingCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToGroupsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToRepositoriesCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToWebsiteCommand { get; }

        public UserViewModel(BitbucketSharp.Models.V2.User user, IApplicationService applicationService = null)
            : this(user.Username, applicationService)
        {
            User = user;
        }

        public UserViewModel(string username, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            GoToFollowersCommand.Subscribe(_ => NavigateTo(new UserFollowersViewModel(username)));
            GoToFollowingCommand.Subscribe(_ => NavigateTo(new UserFollowingsViewModel(username)));
            GoToEventsCommand.Subscribe(_ => NavigateTo(new UserEventsViewModel(username)));
            GoToGroupsCommand.Subscribe(_ => NavigateTo(new GroupsViewModel(username)));
            GoToRepositoriesCommand.Subscribe(_ => NavigateTo(new UserRepositoriesViewModel(username)));

            this.WhenAnyValue(x => x.User.Website)
                .Select(x => !string.IsNullOrEmpty(x))
                .ToProperty(this, x => x.IsWebsiteAvailable, out _isWebsiteAvailable);

            this.WhenAnyValue(x => x.User.DisplayName)
                .Select(x => string.Equals(x, username) ? null : x)
                .ToProperty(this, x => x.DisplayName, out _displayName);

            GoToWebsiteCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.IsWebsiteAvailable));
            GoToWebsiteCommand.Subscribe(_ => NavigateTo(new WebBrowserViewModel(User.Website)));

            ShouldShowGroups = string.Equals(username, applicationService.Account.Username, StringComparison.OrdinalIgnoreCase);

            Title = username;

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => 
            {
                if (!string.Equals(applicationService.Account.Username, username, StringComparison.OrdinalIgnoreCase))
                {
                    applicationService.Client.Groups.GetGroups(username)
                                  .ToObservable()
                                  .Select(_ => true)
                                  .Catch(Observable.Return(false))
                                  .ObserveOn(RxApp.MainThreadScheduler)
                                  .Subscribe(x => ShouldShowGroups = x);
                }

                User = await applicationService.Client.Users.GetUser(username);
            });
        }
    }
}

