using System;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.User
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel
    {
        public CollectionViewModel<BitbucketSharp.Models.V2.User> Users { get; } = new CollectionViewModel<BitbucketSharp.Models.V2.User>();

        public ReactiveUI.IReactiveCommand<object> GoToUserCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        protected BaseUserCollectionViewModel()
        {
            GoToUserCommand
                .OfType<BitbucketSharp.Models.V2.User>()
                .Select(x => new UserViewModel.NavObject { Username = x.Username })
                .Subscribe(x => ShowViewModel<UserViewModel>(x));
        }
    }
}
