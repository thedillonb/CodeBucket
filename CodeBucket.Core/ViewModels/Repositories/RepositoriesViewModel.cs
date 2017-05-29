using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Splat;
using System.Reactive;
using System.Threading.Tasks;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public abstract class RepositoriesViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<RepositoryItemViewModel>
    {
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<RepositoryItemViewModel> Items { get; }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        protected RepositoriesViewModel(IApplicationService applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Repositories";

            var showDescription = applicationService.Account.RepositoryDescriptionInList;
            var repositories = new ReactiveList<Repository>(resetChangeThreshold: 10);

            Items = repositories.CreateDerivedCollection(x =>
            {
                var description = showDescription ? x.Description : string.Empty;
                var viewModel = new RepositoryItemViewModel(x.Name, description, x.Owner?.Username, new Avatar(x.Owner?.Links?.Avatar?.Href));
                viewModel.GoToCommand.Subscribe(_ =>
                {
                    var id = RepositoryIdentifier.FromFullName(x.FullName);
                    NavigateTo(new RepositoryViewModel(id.Owner, id.Name, x));
                });
                return viewModel;
            }, x => x.Name.ContainsKeyword(SearchText), signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                repositories.Clear();
                await Load(applicationService, repositories);
            });

            LoadCommand.IsExecuting.CombineLatest(repositories.IsEmptyChanged, (x, y) => !x && y)
                       .ToProperty(this, x => x.IsEmpty, out _isEmpty);
        }

        protected abstract Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories);
    }
}