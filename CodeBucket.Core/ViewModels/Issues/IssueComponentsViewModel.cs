using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueComponentsViewModel : ReactiveObject, ILoadableViewModel
	{
        public IReadOnlyReactiveList<IssueComponentItemViewModel> Components { get; }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

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

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                components.Reset(await applicationService.Client.Repositories.Issues.GetComponents(username, repository));
            });
        }

        private IssueComponentItemViewModel CreateItemViewModel(IssueComponent component)
        {
            var vm = new IssueComponentItemViewModel(component.Name, string.Equals(SelectedValue, component.Name));
            vm.WhenAnyValue(y => y.IsSelected).Skip(1).Subscribe(y => SelectedValue = y ? component.Name : null);
            return vm;
        }
	}
}

