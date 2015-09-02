using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.User
{
    public abstract class BaseUserCollectionViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<UserModel> _users = new CollectionViewModel<UserModel>();

		public CollectionViewModel<UserModel> Users
        {
            get { return _users; }
        }

        public ICommand GoToUserCommand
        {
			get { return new MvxCommand<UserModel>(x => this.ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x.Username })); }
        }
    }
}
