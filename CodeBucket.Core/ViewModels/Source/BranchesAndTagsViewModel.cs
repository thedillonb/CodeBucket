using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Linq;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Source
{
	public class BranchesAndTagsViewModel : LoadableViewModel
	{
		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public CollectionViewModel<ViewObject> Items { get; }

		public ICommand GoToSourceCommand
		{
			get { return new MvxCommand<ViewObject>(GoToSource); }
		}

		private void GoToSource(ViewObject obj)
		{
			var branch = obj.Object as BranchModel;
			var tag = obj.Object as TagModel;

			if (branch != null)
				ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = branch.Node });
			else if (tag != null)
				ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = tag.Node });
		}

		public BranchesAndTagsViewModel()
		{
            Items = new CollectionViewModel<ViewObject>();
            this.Bind(x => x.SelectedFilter).Subscribe(x => LoadCommand.Execute(false));
		}

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
			_selectedFilter = navObject.IsShowingBranches ? 0 : 1;
		}

		protected override async Task Load()
		{
			if (SelectedFilter == 0)
			{
                var response = await this.GetApplication().Client.Repositories.GetBranches(Username, Repository);
                Items.Items.Reset(response.Values.OrderBy(x => x.Branch).Select(x => new ViewObject { Name = x.Branch, Object = x }));
			}
			else
			{
                var response = await this.GetApplication().Client.Repositories.GetTags(Username, Repository);
                Items.Items.Reset(response.Select(x => new ViewObject { Name = x.Key, Object = x.Value }));
			}
		}

		public class ViewObject
		{
			public string Name { get; set; }
			public object Object { get; set; }
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public bool IsShowingBranches { get; set; }

			public NavObject()
			{
				IsShowingBranches = true;
			}
		}
	}
}

