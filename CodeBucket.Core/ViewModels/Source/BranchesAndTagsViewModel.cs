using System;
using System.Reactive.Linq;
using MvvmCross.Core.ViewModels;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesAndTagsViewModel : BaseViewModel, ILoadableViewModel
	{
        public CollectionViewModel<ViewObject> Items { get; } = new CollectionViewModel<ViewObject>();

		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public ReactiveUI.IReactiveCommand<object> GoToSourceCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public BranchesAndTagsViewModel(IApplicationService applicationService)
		{
            this.Bind(x => x.SelectedFilter).Subscribe(x => LoadCommand.Execute(false));

            GoToSourceCommand.OfType<ViewObject>().Subscribe(obj => 
            {
                var branch = obj.Object as BranchModel;
                var tag = obj.Object as TagModel;

                if (branch != null)
                    ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = branch.Node });
                else if (tag != null)
                    ShowViewModel<SourceTreeViewModel>(new SourceTreeViewModel.NavObject { Username = Username, Repository = Repository, Branch = tag.Node });
            });

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (SelectedFilter == 0)
                {
                    var branches = await applicationService.Client.Repositories.GetBranches(Username, Repository);
                    Items.Items.Reset(branches.Values.OrderBy(x => x.Branch).Select(x => new ViewObject { Name = x.Branch, Object = x }));
                }
                else
                {
                    var branches = await applicationService.Client.Repositories.GetTags(Username, Repository);
                    Items.Items.Reset(branches.Select(x => new ViewObject { Name = x.Key, Object = x.Value }));
                }
            });
		}

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
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
		}
	}
}

