using CodeBucket.Core.Services;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Splat;
using System.Reactive;
using CodeBucket.Client.V1;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueMilestonesViewModel : ReactiveObject, ILoadableViewModel, IViewModel
	{
        private bool _isLoaded;
        public IReadOnlyReactiveList<IssueAttributeItemViewModel> Milestones { get; }

        private string _selectedValue;
		public string SelectedValue
		{
            get { return _selectedValue; }
            set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
		}

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public IssueMilestonesViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var milestones = new ReactiveList<IssueMilestone>();
            Milestones = milestones.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Milestones)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                if (_isLoaded) return;
                milestones.Reset(await applicationService.Client.Issues.GetMilestones(username, repository));
                _isLoaded = true;
            });
        }

        private IssueAttributeItemViewModel CreateItemViewModel(IssueMilestone milestone)
        {
            var vm = new IssueAttributeItemViewModel(milestone.Name, string.Equals(SelectedValue, milestone.Name));
            vm.SelectCommand.Subscribe(y => SelectedValue = !vm.IsSelected ? vm.Name : null);
            vm.SelectCommand.InvokeCommand(DismissCommand);
            return vm;
        }
	}
}

