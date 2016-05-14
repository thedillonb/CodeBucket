using System;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Teams;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Users
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel, IProvidesSearch, IListViewModel<UserItemViewModel>
    {
        private string _emptyMessage;
        public string EmptyMessage
        {
            get { return _emptyMessage; }
            protected set { this.RaiseAndSetIfChanged(ref _emptyMessage, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        protected ReactiveList<UserItemViewModel> Users { get; } = new ReactiveList<UserItemViewModel>();

        public IReadOnlyReactiveList<UserItemViewModel> Items { get; }

        protected BaseUserCollectionViewModel()
        {
            Items = Users.CreateDerivedCollection(
                x => x,
                x => x.Username.ContainsKeyword(SearchText) || x.DisplayName.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));
        }

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
