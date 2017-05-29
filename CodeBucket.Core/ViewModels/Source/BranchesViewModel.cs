using System;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using System.Reactive;
using Splat;
using CodeBucket.Client.V1;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<GitReferenceItemViewModel>
    {
        public IReadOnlyReactiveList<GitReferenceItemViewModel> Items { get; }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        public static BranchesViewModel ForCommits(string username, string repository)
        {
            return new BranchesViewModel(
                username, repository,
                (vm, r) => new CommitsViewModel(username, repository, r.Node));                         
        }

        public static BranchesViewModel ForSource(string username, string repository)
        {
            return new BranchesViewModel(
                username, repository,
                (vm, r) => new SourceTreeViewModel(username, repository, r.Node));
        }

        public BranchesViewModel(
            string username, string repository,
            Func<BranchesViewModel, GitReference, IViewModel> clickFunc,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Branches";

            var branches = new ReactiveList<GitReference>();
            Items = branches.CreateDerivedCollection(
                branch =>
                {
                var vm = new GitReferenceItemViewModel(branch.Name);
                    vm.GoToCommand
                      .Select(_ => clickFunc(this, branch))
                      .Subscribe(NavigateTo);
                    return vm;
                },
                x => x.Branch.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateFromTask(async t =>
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

