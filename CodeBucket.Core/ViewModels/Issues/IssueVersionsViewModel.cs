using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueVersionsViewModel : ReactiveObject, ILoadableViewModel
	{
        private bool _isLoaded;

        public IReadOnlyReactiveList<IssueAttributeItemViewModel> Versions { get; }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public IssueVersionsViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var versions = new ReactiveList<IssueVersion>();
            Versions = versions.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Versions)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (_isLoaded) return;
                var items = await applicationService.Client.Repositories.Issues.GetVersions(username, repository);
                versions.Reset(items);
                _isLoaded = true;
            });
        }

        private IssueAttributeItemViewModel CreateItemViewModel(IssueVersion version)
        {
            var vm = new IssueAttributeItemViewModel(version.Name, string.Equals(SelectedValue, version.Name));
            vm.SelectCommand.Subscribe(y => SelectedValue = !vm.IsSelected ? vm.Name : null);
            vm.SelectCommand.InvokeCommand(DismissCommand);
            return vm;
        }
	}
}

