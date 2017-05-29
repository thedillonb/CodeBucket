using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive;
using Splat;
using CodeBucket.Client.V1;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueComponentsViewModel : ReactiveObject, ILoadableViewModel
	{
        private bool _isLoaded;
        public IReadOnlyReactiveList<IssueAttributeItemViewModel> Components { get; }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> DismissCommand { get; } = ReactiveCommandFactory.Empty();

        public IssueComponentsViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var components = new ReactiveList<IssueComponent>();
            Components = components.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Components)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));

            LoadCommand = ReactiveCommand.CreateFromTask(async _ => {
                if (_isLoaded) return;
                var items = await applicationService.Client.Issues.GetComponents(username, repository);
                components.Reset(items);
                _isLoaded = true;
            });
        }

        private IssueAttributeItemViewModel CreateItemViewModel(IssueComponent component)
        {
            var vm = new IssueAttributeItemViewModel(component.Name, string.Equals(SelectedValue, component.Name));
            vm.SelectCommand.Subscribe(y => SelectedValue = !vm.IsSelected ? vm.Name : null);
            vm.SelectCommand.BindCommand(DismissCommand);
            return vm;
        }
	}
}

