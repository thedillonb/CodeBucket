using System;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using System.Reactive;
using Splat;
using CodeBucket.Client.V1;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<GitReferenceItemViewModel>
    {
        public IReadOnlyReactiveList<GitReferenceItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        public BranchesViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Branches";

            var branches = new ReactiveList<GitReference>();
            Items = branches.CreateDerivedCollection(
                branch =>
                {
                var vm = new GitReferenceItemViewModel(branch.Branch);
                    vm.GoToCommand
                      .Select(_ => new CommitsViewModel(username, repository, branch.Node))
                      .Subscribe(NavigateTo);
                    return vm;
                },
                x => x.Branch.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                branches.Clear();
                var items = await applicationService.Client.Repositories.GetBranches(username, repository);
                branches.AddRange(items);
            });

            LoadCommand.IsExecuting.CombineLatest(branches.IsEmptyChanged, (x, y) => !x && y)
                       .ToProperty(this, x => x.IsEmpty, out _isEmpty);
        }
    }
}

