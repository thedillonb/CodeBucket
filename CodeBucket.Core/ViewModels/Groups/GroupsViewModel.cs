using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Groups
{
	public class GroupsViewModel : LoadableViewModel
	{
		private readonly CollectionViewModel<GroupModel> _orgs = new CollectionViewModel<GroupModel>();

		public CollectionViewModel<GroupModel> Organizations
        {
            get { return _orgs; }
        }

        public string Username 
        { 
            get; 
            private set; 
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        public ICommand GoToGroupCommand
        {
			get { return new MvxCommand<GroupModel>(x => ShowViewModel<GroupViewModel>(new GroupViewModel.NavObject { Username = x.Owner.Username, GroupName = x.Name }));}
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Organizations.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Groups.GetGroups(forceCacheInvalidation).OrderBy(a => a.Name).ToList());
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
	}
}

