using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using System.Collections.Generic;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueComponentsViewModel : ReactiveObject, ILoadableViewModel
	{
        private IList<IssueComponent> _components;
        public IList<IssueComponent> Components
        {
            get { return _components; }
            private set { this.RaiseAndSetIfChanged(ref _components, value); }
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

        public IssueComponentsViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                Components = await applicationService.Client.Repositories.Issues.GetComponents(Username, Repository);
            });
        }

        public void Init(string username, string repository)
        {
            Username = username;
            Repository = repository;
        }
	}
}

