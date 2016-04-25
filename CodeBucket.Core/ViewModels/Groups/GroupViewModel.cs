using System;
using CodeBucket.Core.ViewModels.Users;
using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using CodeBucket.Core.Utils;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupViewModel : BaseUserCollectionViewModel, ILoadableViewModel
	{
        private string _slug;

        public string Owner { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public GroupViewModel(IApplicationService applicationService)
        {
            Title = "Members";
            EmptyMessage = "There are no members.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var members = await applicationService.Client.Groups.GetMembers(Owner, _slug);
                var memberUsers = members.Select(x =>
                {
                    var username = x.Username;
                    var avatar = new Avatar(x.Avatar);
                    var displayName = string.Join(" ", x.FirstName, x.LastName);
                    var vm = new UserItemViewModel(username, displayName, avatar);
                    vm.GoToCommand
                      .Select(__ => new UserViewModel.NavObject { Username = username })
                      .Subscribe(y => ShowViewModel<UserViewModel>(x));
                    return vm;
                });

                Users.Reset(memberUsers);
            });
        }

		public void Init(NavObject navObject) 
		{
			Owner = navObject.Owner;
			Title = navObject.Name;
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

