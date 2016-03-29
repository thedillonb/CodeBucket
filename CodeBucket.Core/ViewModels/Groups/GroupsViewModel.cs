using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupsViewModel : BaseViewModel, ILoadableViewModel
	{
        public CollectionViewModel<GroupModel> Groups { get; } = new CollectionViewModel<GroupModel>();

        public string Username { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public IReactiveCommand<object> GoToGroupCommand { get; }

        public GroupsViewModel(IApplicationService applicationService)
        {
            GoToGroupCommand = ReactiveCommand.Create();
            GoToGroupCommand
                .OfType<GroupModel>()
                .Select(x => new GroupViewModel.NavObject { Username = x.Owner.Username, GroupName = x.Name })
                .Subscribe(x => ShowViewModel<GroupViewModel>(x));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
                Groups.Items.Reset(await applicationService.Client.Groups.GetGroups(Username));
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
	}
}

