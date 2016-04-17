using CodeBucket.Core.ViewModels.User;
using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupViewModel : BaseUserCollectionViewModel, ILoadableViewModel
	{
        private string _slug;

        public string Owner { get; private set; }

		public string Name { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public GroupViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var members = await applicationService.Client.Groups.GetMembers(Owner, _slug);
                var memberUsers = members.Select(x => new BitbucketSharp.Models.V2.User
                {
                    Username = x.Username,
                    Links = new BitbucketSharp.Models.V2.User.LinksModel { Avatar = new BitbucketSharp.Models.V2.LinkModel { Href = x.Avatar } }
                });

                Users.Items.Reset(memberUsers);
            });
        }

		public void Init(NavObject navObject) 
		{
			Owner = navObject.Owner;
			Name = navObject.Name;
            _slug = navObject.Slug;
		}

        public class NavObject
        {
			public string Owner { get; set; }
			public string Name { get; set; }
            public string Slug { get; set; }
        }
	}
}

