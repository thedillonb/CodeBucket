using System;
using System.Reactive.Linq;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesAndTagsViewModel : BaseViewModel, ILoadableViewModel
	{
		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<ReferenceItemViewModel> Items { get; }

        public BranchesAndTagsViewModel(
            string username, string repository, 
            IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            this.WhenAnyValue(x => x.SelectedFilter)
                .Skip(1)
                .InvokeCommand(LoadCommand);

            var items = new ReactiveList<ReferenceModel>();
            Items = items.CreateDerivedCollection(x =>
            {
                var vm = new ReferenceItemViewModel(x.Branch);
                vm.GoToCommand
                  .Select(_ => new SourceTreeViewModel(username, repository, x.Node))
                  .Subscribe(NavigateTo);
                return vm;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                items.Reset();

                if (SelectedFilter == 0)
                {
                    var branches = await applicationService.Client.Repositories.GetBranches(username, repository);
                    items.Reset(branches.Select(x => x.Value));
                }
                else
                {
                    var tags = await applicationService.Client.Repositories.GetTags(username, repository);
                    items.Reset(tags.Select(x => x.Value));
                }
            });
		}
	}
}

