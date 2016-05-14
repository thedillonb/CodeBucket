using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public abstract class RepositoriesViewModel : BaseViewModel, IProvidesSearch, IListViewModel<RepositoryItemViewModel>
    {
        protected readonly IReactiveList<Repository> RepositoryList = new ReactiveList<Repository>();

        public IReadOnlyReactiveList<RepositoryItemViewModel> Items { get; }

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
            Items = RepositoryList.CreateDerivedCollection(x =>
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
        }
    }
}