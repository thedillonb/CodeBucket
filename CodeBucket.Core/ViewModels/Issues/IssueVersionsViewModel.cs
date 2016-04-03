using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeBucket.Core.Services;
using ReactiveUI;

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

		public string Username  { get; private set; }

		public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public IssueVersionsViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                Versions = await applicationService.Client.Repositories.Issues.GetVersions(Username, Repository);
            });
        }

        public void Init(string username, string repository)
        {
            Username = username;
            Repository = repository;
        }
	}
}

