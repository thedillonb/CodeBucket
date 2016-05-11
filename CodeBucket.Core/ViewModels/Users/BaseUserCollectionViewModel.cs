using System;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Teams;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Users
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel
    {
        private string _emptyMessage;
        public string EmptyMessage
        {
            get { return _emptyMessage; }
            protected set { this.RaiseAndSetIfChanged(ref _emptyMessage, value); }
        }

        public ReactiveList<UserItemViewModel> Users { get; } = new ReactiveList<UserItemViewModel>();

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
