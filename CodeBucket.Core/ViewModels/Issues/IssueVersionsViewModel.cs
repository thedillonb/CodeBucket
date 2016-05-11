using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeBucket.Core.Services;
using ReactiveUI;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueVersionsViewModel : ReactiveObject, ILoadableViewModel
	{
        private IList<IssueVersion> _versions;
        public IList<IssueVersion> Versions
        {
            get { return _versions; }
            private set { this.RaiseAndSetIfChanged(ref _versions, value); }
        }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            private set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IssueVersionsViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                Versions = await applicationService.Client.Repositories.Issues.GetVersions(username, repository);
            });
        }
	}
}

