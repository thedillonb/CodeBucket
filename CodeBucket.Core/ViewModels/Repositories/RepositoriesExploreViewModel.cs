using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Repositories;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels
{
    public class RepositoriesExploreViewModel : BaseViewModel
    {
		private readonly CollectionViewModel<RepositoryDetailedModel> _repositories = new CollectionViewModel<RepositoryDetailedModel>();
        private string _searchText;

        public bool ShowRepositoryDescription
        {
			get { return !this.GetApplication().Account.HideRepositoryDescriptionInList; }
        }

		public CollectionViewModel<RepositoryDetailedModel> Repositories
        {
            get { return _repositories; }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(() => SearchText); }
        }

		public ICommand GoToRepositoryCommand
		{
			get { return new MvxCommand<RepositoryDetailedModel>(x => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, Repository = x.Name })); }
		}

        public ICommand SearchCommand
        {
			get { return new MvxCommand(Search, () => !string.IsNullOrEmpty(SearchText)); }
        }

        private async void Search()
        {
            await Task.Run(() =>
            {
				Repositories.Items.Reset(this.GetApplication().Client.Repositories.Search(SearchText).Repositories);
            });
        }
    }
}

