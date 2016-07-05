using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using CodeBucket.Core.Utils;
using Splat;
using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesExploreViewModel : BaseViewModel, IListViewModel<RepositoryItemViewModel>
    {
        public IReadOnlyReactiveList<RepositoryItemViewModel> Items { get; }

		private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public IReactiveCommand<Unit> SearchCommand { get; }

        public bool IsEmpty => false;

        public RepositoriesExploreViewModel(
            IApplicationService applicationService = null,
            ILoadingIndicatorService loadingIndicatorService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            loadingIndicatorService = loadingIndicatorService ?? Locator.Current.GetService<ILoadingIndicatorService>();

            Title = "Explore";

            var showDescription = applicationService.Account.RepositoryDescriptionInList;

            var repositories = new ReactiveList<Client.V1.Repository>();
            Items = repositories.CreateDerivedCollection(x =>
            {
                var description = showDescription ? x.Description : string.Empty;
                var vm = new RepositoryItemViewModel(x.Name, description, x.Owner, new Avatar(x.Logo));
                vm.GoToCommand
                  .Select(_ => new RepositoryViewModel(x.Owner, x.Slug))
                  .Subscribe(NavigateTo);
                return vm;
            });

            var canSearch = this.WhenAnyValue(x => x.SearchText).Select(x => !string.IsNullOrEmpty(x));
            SearchCommand = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>
            {
                repositories.Clear();

                if (string.IsNullOrEmpty(SearchText))
                    return;

                var client = new HttpClient(new LoadingMessageHandler(loadingIndicatorService));
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var resp = await client.GetAsync("https://bitbucket.org/xhr/repos?term=" + Uri.EscapeDataString(SearchText));
                var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                var repos = JsonConvert.DeserializeObject<List<RepositorySearch>>(body);

                repositories.Reset(repos.Select(x =>
                {
                    return new Client.V1.Repository
                    {
                        Name = x.Slug,
                        Description = x.Name,
                        Owner = x.Owner,
                        Logo = x.Avatar,
                        Website = x.Href,
                        Slug = x.Slug
                    };
                }));
            });
        }

        private class RepositorySearch
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("full_slug")]
            public string FullSlug { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("href")]
            public string Href { get; set; }

            [JsonProperty("owner")]
            public string Owner { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }
        }
    }
}

