using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Groups
{
	public class GroupsViewModel : LoadableViewModel
	{
        public CollectionViewModel<GroupModel> Groups { get; } = new CollectionViewModel<GroupModel>();

        public string Username { get; private set; }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        public ICommand GoToGroupCommand
        {
			get { return new MvxCommand<GroupModel>(x => ShowViewModel<GroupViewModel>(new GroupViewModel.NavObject { Username = x.Owner.Username, GroupName = x.Name }));}
        }

        protected override async Task Load()
        {
            var response = await this.GetApplication().Client.Groups.GetGroups(Username);
            Groups.Items.Reset(response);
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
	}
}

