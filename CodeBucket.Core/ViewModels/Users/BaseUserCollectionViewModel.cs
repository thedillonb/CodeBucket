using System;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.ViewModels.Users
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel
    {
        public CollectionViewModel<UserItemViewModel> Users { get; } = new CollectionViewModel<UserItemViewModel>();

        protected UserItemViewModel ToViewModel(User user)
        {
            var avatar = new Avatar(user.Links?.Avatar?.Href);
            var vm = new UserItemViewModel(user.Username, user.DisplayName, avatar);
            vm.GoToCommand
              .Select(x => new UserViewModel.NavObject { Username = user.Username })
              .Subscribe(x => ShowViewModel<UserViewModel>(x));
            return vm;
        }
    }
}
