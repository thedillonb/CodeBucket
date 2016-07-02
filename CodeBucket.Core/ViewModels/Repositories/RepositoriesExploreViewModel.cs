using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Client.Models;
using System;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesExploreViewModel : BaseViewModel
    {
        public bool ShowRepositoryDescription
        {
			get { return this.GetApplication().Account.RepositoryDescriptionInList; }
        }

		private readonly CollectionViewModel<RepositoryDetailedModel> _repositories = new CollectionViewModel<RepositoryDetailedModel>();
		public CollectionViewModel<RepositoryDetailedModel> Repositories
        {
            get { return _repositories; }
        }

		private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(() => SearchText); }
        }

		private bool _isSearching;
		public bool IsSearching
		{
			get { return _isSearching; }
			private set
			{
				_isSearching = value;
				RaisePropertyChanged(() => IsSearching);
			}
		}

		public ICommand GoToRepositoryCommand
		{
			get { return new MvxCommand<RepositoryDetailedModel>(x => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, RepositorySlug = x.Slug })); }
		}

        public ICommand SearchCommand
        {
            get { return new MvxCommand(() => Search(), () => !string.IsNullOrEmpty(SearchText)); }
        }

		public RepositoriesExploreViewModel()
		{
			_repositories.SortingFunction = x => x.OrderByDescending(y => y.FollowersCount);
		}

        private async Task Search()
        {
			try
			{
				IsSearching = true;
				var data = await this.GetApplication().Client.Repositories.Search(SearchText);
                Repositories.Items.Reset(data.Repositories);
			}
			catch (Exception e)
			{
                DisplayAlert("Failed to retrieve list of repositories. " + e.Message).ToBackground();
			}
			finally
			{
				IsSearching = false;
			}
        }
    }
}

