using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueMilestonesViewModel : ReactiveObject, ILoadableViewModel
	{
        public IReadOnlyReactiveList<IssueMilestoneItemViewModel> Milestones { get; }

        private string _selectedValue;
		public string SelectedValue
		{
            get { return _selectedValue; }
            private set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
		}

		public string Username  { get; private set; }

		public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public IssueMilestonesViewModel(IApplicationService applicationService)
        {
            var milestones = new ReactiveList<IssueMilestone>();
            Milestones = milestones.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Milestones)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                milestones.Reset(await applicationService.Client.Repositories.Issues.GetMilestones(Username, Repository));
            });
        }

        private IssueMilestoneItemViewModel CreateItemViewModel(IssueMilestone component)
        {
            var vm = new IssueMilestoneItemViewModel(component.Name, string.Equals(SelectedValue, component.Name));
            vm.WhenAnyValue(y => y.IsSelected).Skip(1).Subscribe(y => SelectedValue = y ? component.Name : null);
            return vm;
        }

        public void Init(string username, string repository)
		{
            Username = username;
            Repository = repository;
		}
	}
}

