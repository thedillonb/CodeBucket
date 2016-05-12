using System.Threading.Tasks;
using BitbucketSharp.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using CodeBucket.Core.Utils;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesExploreViewModel : BaseViewModel
    {
        public IReadOnlyReactiveList<RepositoryItemViewModel> Repositories { get; }

		private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public IReactiveCommand<Unit> SearchCommand { get; }

        public IReactiveCommand<object> GoToRepositoryCommand { get; } = ReactiveCommand.Create();

		public RepositoriesExploreViewModel(IApplicationService applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Explore";

            var canSearch = this.WhenAnyValue(x => x.SearchText).Select(x => !string.IsNullOrEmpty(x));
            SearchCommand = ReactiveCommand.CreateAsyncTask(canSearch, _ => Search());
            var showDescription = applicationService.Account.RepositoryDescriptionInList;

            var repositories = new ReactiveList<RepositoryDetailedModel>();
            Repositories = repositories.CreateDerivedCollection(x =>
            {
                var description = showDescription ? x.Description : string.Empty;
                return new RepositoryItemViewModel(x.Name, description, x.Owner, new Avatar(x.Logo));
            });
        }

        private async Task Search()
        {
//			try
//			{
//				IsSearching = true;
//				var data = await Task.Run(() => this.GetApplication().Client.Repositories.Search(SearchText).Repositories);
//                Repositories.Items.Reset(data);
//			}
//			catch (Exception e)
//			{
//                DisplayAlert("Failed to retrieve list of repositories. " + e.Message);
//			}
//			finally
//			{
//				IsSearching = false;
//			}
        }
    }
}

