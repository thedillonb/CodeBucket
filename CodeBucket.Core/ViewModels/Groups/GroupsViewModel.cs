using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupsViewModel : BaseViewModel, ILoadableViewModel
	{
        public CollectionViewModel<GroupItemViewModel> Groups { get; } = new CollectionViewModel<GroupItemViewModel>();

        public string Username { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public GroupsViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => {
                var groups = await applicationService.Client.Groups.GetGroups(Username);
                Groups.Items.Reset(groups.Select(ToViewModel));
            });
        }

        private GroupItemViewModel ToViewModel(GroupModel model)
        {
            var vm = new GroupItemViewModel(model.Name);
            vm.GoToCommand
              .Select(x => new GroupViewModel.NavObject { Owner = model.Owner.Username, Name = model.Name, Slug = model.Slug })
              .Subscribe(x => ShowViewModel<GroupViewModel>(x));
            return vm;
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

