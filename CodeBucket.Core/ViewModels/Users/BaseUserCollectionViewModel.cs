using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Users
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
