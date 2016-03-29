using CodeBucket.Core.ViewModels.User;
using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupViewModel : BaseUserCollectionViewModel, ILoadableViewModel
	{
		public string Username { get; private set; }

		public string GroupName { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public GroupViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var members = await applicationService.Client.Groups.GetMembers(Username, GroupName);
                var memberUsers = members.Select(x => new BitbucketSharp.Models.V2.User {
                    Username = x.Username,
                    Links = new BitbucketSharp.Models.V2.User.LinksModel { Avatar = new BitbucketSharp.Models.V2.LinkModel { Href = x.Avatar } }
                });

                Users.Items.Reset(memberUsers);
            });
        }

		public void Init(NavObject navObject) 
		{
			Username = navObject.Username;
			GroupName = navObject.GroupName;
		}

        public class NavObject
        {
			public string Username { get; set; }
			public string GroupName { get; set; }
        }
	}
}

