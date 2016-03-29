using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.User
{
    public abstract class BaseUserCollectionViewModel : BaseViewModel
    {
        public CollectionViewModel<BitbucketSharp.Models.V2.User> Users { get; } = new CollectionViewModel<BitbucketSharp.Models.V2.User>();

        public ICommand GoToUserCommand
        {
            get { return new MvxCommand<BitbucketSharp.Models.V2.User>(x => this.ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x.Username })); }
        }
    }
}
