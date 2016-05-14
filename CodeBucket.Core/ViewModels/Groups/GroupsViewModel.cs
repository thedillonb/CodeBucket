using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupsViewModel : BaseViewModel, ILoadableViewModel, IProvidesSearch, IListViewModel<GroupItemViewModel>
	{
        public IReadOnlyReactiveList<GroupItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public GroupsViewModel(
            string username, 
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Groups";

            var groups = new ReactiveList<GroupModel>();
            Items = groups.CreateDerivedCollection(
                ToViewModel,
                x => x.Name.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => {
                groups.Reset(await applicationService.Client.Groups.GetGroups(username));
            });
        }

        private GroupItemViewModel ToViewModel(GroupModel model)
        {
            var vm = new GroupItemViewModel(model.Name);
            vm.GoToCommand
              .Select(x => new GroupViewModel(model.Owner.Username, model.Slug, model.Name))
              .Subscribe(NavigateTo);
            return vm;
        }
	}
}

