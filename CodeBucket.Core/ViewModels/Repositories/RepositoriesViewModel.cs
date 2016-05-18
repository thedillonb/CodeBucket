using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Splat;
using System.Reactive;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public abstract class RepositoriesViewModel : BaseViewModel, IProvidesSearch, ILoadableViewModel, IListViewModel<RepositoryItemViewModel>
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<RepositoryItemViewModel> Items { get; }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            private set { this.RaiseAndSetIfChanged(ref _isEmpty, value); }
        }

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
            var repositories = new ReactiveList<Repository>();

            Items = repositories.CreateDerivedCollection(x =>
            {
                var description = showDescription ? x.Description : string.Empty;
                var viewModel = new RepositoryItemViewModel(x.Name, description, x.Owner?.Username, new Avatar(x.Owner?.Links?.Avatar?.Href));
                viewModel.GoToCommand.Subscribe(_ =>
                {
                    var id = RepositoryIdentifier.FromFullName(x.FullName);
                    NavigateTo(new RepositoryViewModel(id.Owner, id.Name));
                });
                return viewModel;
            }, x => x.Name.ContainsKeyword(SearchText), signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                repositories.Clear();
                await Load(applicationService, repositories);
            });

            LoadCommand
                .IsExecuting
                .Subscribe(x => IsEmpty = !x && repositories.Count == 0);
        }

        protected abstract Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories);
    }
}