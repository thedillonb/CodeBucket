using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Core.ViewModels
{
    public class TeamsViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<string> _teams = new CollectionViewModel<string>();

		public CollectionViewModel<string> Teams
        {
            get { return _teams; }
        }

        public ICommand GoToTeamCommand
        {
			get { return new MvxCommand<string>(x => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x })); }
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Teams.SimpleCollectionLoad(() => this.GetApplication().Client.Account.GetPrivileges(forceCacheInvalidation).Teams.Keys.OrderBy(a => a).ToList());
        }
    }
}