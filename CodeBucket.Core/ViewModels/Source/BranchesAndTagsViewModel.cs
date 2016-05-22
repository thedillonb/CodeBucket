using System;
using System.Reactive.Linq;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesAndTagsViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<ReferenceItemViewModel>
	{
		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            private set { this.RaiseAndSetIfChanged(ref _isEmpty, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<ReferenceItemViewModel> Items { get; }

        public BranchesAndTagsViewModel(
            string username, string repository, 
            IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var items = new ReactiveList<Tuple<string, ReferenceModel>>();
            Items = items.CreateDerivedCollection(
                x =>
                {
                    var vm = new ReferenceItemViewModel(x.Item1);
                    vm.GoToCommand
                      .Select(_ => new SourceTreeViewModel(username, repository, x.Item2.Node))
                      .Subscribe(NavigateTo);
                    return vm;
                },
                x => x.Item1.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                items.Clear();

                if (SelectedFilter == 0)
                {
                    var branches = await applicationService.Client.Repositories.GetBranches(username, repository);
                    items.Reset(branches.Select(x => Tuple.Create(x.Key, x.Value)));
                }
                else
                {
                    var tags = await applicationService.Client.Repositories.GetTags(username, repository);
                    items.Reset(tags.Select(x => Tuple.Create(x.Key, x.Value)));
                }
            });

            LoadCommand
                .IsExecuting
                .Subscribe(x => IsEmpty = !x && items.Count == 0);

            this.WhenAnyValue(x => x.SelectedFilter)
                .Skip(1)
                .InvokeCommand(LoadCommand);
        }
	}
}

