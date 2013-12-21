using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Source
{
	public class BranchesAndTagsViewModel : LoadableViewModel
	{
		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
			set 
			{
				_selectedFilter = value;
				RaisePropertyChanged(() => SelectedFilter);
			}
		}

		public string Username
		{
			get;
			private set;
		}

		public string Repository
		{
			get;
			private set;
		}

		private readonly CollectionViewModel<ViewObject> _items = new CollectionViewModel<ViewObject>();
		public CollectionViewModel<ViewObject> Items
		{
			get { return _items; }
		}

		public ICommand GoToSourceCommand
		{
			get { return new MvxCommand<ViewObject>(GoToSource); }
		}

		private void GoToSource(ViewObject obj)
		{
			if (obj.Object is BranchModel)
			{
				var x = obj.Object as BranchModel;
				ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = x.Node });
			}
			else if (obj.Object is TagModel)
			{
				var x = obj.Object as TagModel;
				ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = x.Node });
			}
		}

		public BranchesAndTagsViewModel()
		{
			this.Bind(x => x.SelectedFilter, x => LoadCommand.Execute(false));
		}

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
			_selectedFilter = navObject.IsShowingBranches ? 0 : 1;
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
			if (SelectedFilter == 0)
			{
				return this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Branches.GetBranches(forceCacheInvalidation), response =>
				{
						//this.CreateMore(response, m => Items.MoreItems = m, d => Items.Items.AddRange(d.Where(x => x != null).Select(x => new ViewObject { Name = x.Name, Object = x })));
						Items.Items.Reset(response.Values.OrderBy(x => x.Branch).Select(x => new ViewObject { Name = x.Branch, Object = x }));
				});
			}
			else
			{
				return this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].GetTags(forceCacheInvalidation), response => 
				{
						//this.CreateMore(response, m => Items.MoreItems = m, d => Items.Items.AddRange(d.Where(x => x != null).Select(x => new ViewObject { Name = x.Name, Object = x })));
						Items.Items.Reset(response.Select(x => new ViewObject { Name = x.Key, Object = x }));
				});
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

