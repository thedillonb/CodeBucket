using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesAndTagsViewModel : BaseViewModel
	{
        public string Username { get; }

        public string Repository { get; }

        public BranchesViewModel BranchesViewModel => BranchesViewModel.ForSource(Username, Repository);

        public TagsViewModel TagsViewModel => TagsViewModel.ForSource(Username, Repository);

		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public BranchesAndTagsViewModel(string username, string repository)
		{
            Username = username;
            Repository = repository;
        }
	}
}

