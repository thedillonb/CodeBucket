using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeBucket.Core.Services;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueMilestonesViewModel : ReactiveObject, ILoadableViewModel
	{
        private IList<IssueMilestone> _milestones;
        public IList<IssueMilestone> Milestones
        {
            get { return _milestones; }
            private set { this.RaiseAndSetIfChanged(ref _milestones, value); }
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

        public IssueMilestonesViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Milestones = await applicationService.Client.Issues.GetMilestones(Username, Repository);
            });
        }

        public void Init(string username, string repository)
		{
            Username = username;
            Repository = repository;
		}
	}
}

