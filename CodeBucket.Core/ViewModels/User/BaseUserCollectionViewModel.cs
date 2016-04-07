using System;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.User
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel
    {
        public CollectionViewModel<BitbucketSharp.Models.V2.User> Users { get; } = new CollectionViewModel<BitbucketSharp.Models.V2.User>();

        public ReactiveUI.IReactiveCommand<object> GoToUserCommand { get; }

        protected BaseUserCollectionViewModel()
        {
            GoToUserCommand = ReactiveUI.ReactiveCommand.Create();
            GoToUserCommand
                .OfType<BitbucketSharp.Models.V2.User>()
                .Select(x => new ProfileViewModel.NavObject { Username = x.Username })
                .Subscribe(x => ShowViewModel<ProfileViewModel>(x));
        }
    }
}
