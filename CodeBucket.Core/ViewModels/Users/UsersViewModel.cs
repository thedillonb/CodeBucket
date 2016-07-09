using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeBucket.Client;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Teams;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Users
{
    public abstract class UsersViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<UserItemViewModel>
    {
        private string _emptyMessage;
        public string EmptyMessage
        {
            get { return _emptyMessage; }
            protected set { this.RaiseAndSetIfChanged(ref _emptyMessage, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<UserItemViewModel> Items { get; }

        protected UsersViewModel()
        {
            var users = new ReactiveList<UserItemViewModel>(resetChangeThreshold: 1);
            Items = users.CreateDerivedCollection(
                x => x,
                x => x.Username.ContainsKeyword(SearchText) || x.DisplayName.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                users.Clear();
                await Load(users);
            });

            _isEmpty = LoadCommand
                .IsExecuting
                .Skip(1)
                .Select(x => !x && users.Count == 0)
                .ToProperty(this, x => x.IsEmpty);
        }

        protected abstract Task Load(ReactiveList<UserItemViewModel> users);

        protected UserItemViewModel ToViewModel(User user)
        {
            var avatar = new Avatar(user.Links?.Avatar?.Href);
            var username = user.Username;
            var vm = new UserItemViewModel(user.Username, user.DisplayName, avatar);
            if (user.Type == "team")
            {
                vm.GoToCommand.Subscribe(_ => NavigateTo(new TeamViewModel(username)));
            }
            else
            {
                vm.GoToCommand.Subscribe(_ => NavigateTo(new UserViewModel(username)));
            }
            return vm;
        }
    }
}
