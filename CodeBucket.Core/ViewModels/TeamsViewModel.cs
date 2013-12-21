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

        public string OrganizationName
        {
            get;
            private set;
        }

        public ICommand GoToTeamCommand
        {
			get { return new MvxCommand<string>(x => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x })); }
        }

        public void Init(NavObject navObject)
        {
            OrganizationName = navObject.Name;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return null;
			//return Teams.SimpleCollectionLoad(() => this.GetApplication().Client.Account.GetPrivileges(forceCacheInvalidation).Teams.Keys.OrderBy(a => a).ToList());
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}