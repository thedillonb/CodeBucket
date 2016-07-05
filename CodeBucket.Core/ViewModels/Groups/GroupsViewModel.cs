using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive;
using Splat;
using CodeBucket.Client.V1;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupsViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<GroupItemViewModel>
	{
        public IReadOnlyReactiveList<GroupItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        public GroupsViewModel(
            string username, 
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Groups";

            var groups = new ReactiveList<Group>();
            Items = groups.CreateDerivedCollection(
                ToViewModel,
                x => x.Name.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => {
                groups.Reset(await applicationService.Client.Groups.GetGroups(username));
            });

            _isEmpty = LoadCommand
                .IsExecuting
                .Skip(1)
                .Select(x => !x && groups.Count == 0)
                .ToProperty(this, x => x.IsEmpty);
        }

        private GroupItemViewModel ToViewModel(Group model)
        {
            var vm = new GroupItemViewModel(model.Name);
            vm.GoToCommand
              .Select(x => new GroupViewModel(model.Owner.Username, model.Slug, model.Name))
              .Subscribe(NavigateTo);
            return vm;
        }
	}
}

