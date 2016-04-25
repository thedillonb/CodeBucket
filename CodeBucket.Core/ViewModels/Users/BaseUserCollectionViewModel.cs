using System;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using CodeBucket.Core.ViewModels.Teams;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.Users
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel, IProvidesTitle
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            protected set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        private string _emptyMessage;
        public string EmptyMessage
        {
            get { return _emptyMessage; }
            protected set { this.RaiseAndSetIfChanged(ref _emptyMessage, value); }
        }

        public ReactiveUI.ReactiveList<UserItemViewModel> Users { get; } = new ReactiveUI.ReactiveList<UserItemViewModel>();

        protected UserItemViewModel ToViewModel(User user)
        {
            var avatar = new Avatar(user.Links?.Avatar?.Href);
            var vm = new UserItemViewModel(user.Username, user.DisplayName, avatar);
            if (user.Type == "team")
            {
                vm.GoToCommand
                  .Select(x => new TeamViewModel.NavObject { Name = user.Username })
                  .Subscribe(x => ShowViewModel<TeamViewModel>(x));
            }
            else
            {
                vm.GoToCommand
                  .Select(x => new UserViewModel.NavObject { Username = user.Username })
                  .Subscribe(x => ShowViewModel<UserViewModel>(x));
            }
            return vm;
        }
    }
}
