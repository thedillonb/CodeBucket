using System.Threading.Tasks;
using BitbucketSharp.Models;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Core.Services;
using Splat;
using System.Collections.Immutable;

namespace CodeBucket.Core.ViewModels.Issues
{
    public abstract class IssueModifyViewModel : BaseViewModel, ILoadableViewModel
    {
		private string _title;
		private string _content;
		private bool _isSaving;

		public string IssueTitle
		{
			get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
		}

		public string Content
		{
			get { return _content; }
            set { this.RaiseAndSetIfChanged(ref _content, value); }
		}

        public IssueMilestonesViewModel Milestones { get; }

        public IssueComponentsViewModel Components { get; }

        public IssueVersionsViewModel Versions { get; }

        public IssueAssigneeViewModel Assignee { get; }

		private string _kind;
		public string Kind
		{
			get { return _kind; }
            set { this.RaiseAndSetIfChanged(ref _kind, value); }
		}

		private string _priority;
		public string Priority
		{
			get { return _priority; }
            set { this.RaiseAndSetIfChanged(ref _priority, value); }
		}
       
		public bool IsSaving
		{
			get { return _isSaving; }
            set { this.RaiseAndSetIfChanged(ref _isSaving, value); }
		}

        public IReactiveCommand<object> GoToMilestonesCommand { get; } = ReactiveCommand.Create();
        public IReactiveCommand<object> GoToVersionsCommand { get; } = ReactiveCommand.Create();
        public IReactiveCommand<object> GoToComponentsCommand { get; } = ReactiveCommand.Create();
        public IReactiveCommand<object> GoToAssigneeCommand { get; } = ReactiveCommand.Create();


        public IReactiveCommand<Unit> SaveCommand { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<ImmutableList<IssueMilestone>> LoadMilestones { get; }

        public string Username { get; }

        public string Repository { get; }

        protected IssueModifyViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Username = username;
            Repository = repository;
            Kind = "bug";
            Priority = "major";

            Milestones = new IssueMilestonesViewModel(username, repository, applicationService);
            Versions = new IssueVersionsViewModel(username, repository, applicationService);
            Components = new IssueComponentsViewModel(username, repository, applicationService);
            Assignee = new IssueAssigneeViewModel(username, repository, applicationService); 

            LoadMilestones = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var items = await applicationService.Client.Repositories.Issues.GetMilestones(username, repository);
                return ImmutableList.CreateRange(items);
            });


            SaveCommand = ReactiveCommand.CreateAsyncTask(t => Save());

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
            });
        }

		protected Task Load()
		{
			return Task.FromResult(false);
		}

		protected abstract Task Save();
    }
}

