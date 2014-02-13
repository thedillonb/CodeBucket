using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Teams
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
            get { return new MvxCommand<string>(x => ShowViewModel<TeamViewModel>(new TeamViewModel.NavObject { Name = x })); }
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Teams.SimpleCollectionLoad(() => this.GetApplication().Client.Account.GetPrivileges(forceCacheInvalidation).Teams.Keys.OrderBy(a => a).ToList());
        }
    }
}