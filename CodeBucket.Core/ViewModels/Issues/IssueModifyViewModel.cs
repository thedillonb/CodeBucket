using System.Threading.Tasks;
using BitbucketSharp.Models;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.ViewModels.Issues
{
    public abstract class IssueModifyViewModel : BaseViewModel, ILoadableViewModel
    {
		private string _title;
		private string _content;
		private UserModel _assignedTo;
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

		private string _milestone;
		public string Milestone
		{
			get { return _milestone; }
            set { this.RaiseAndSetIfChanged(ref _milestone, value); }
		}

		private bool _milestonesAvailable;
		public bool MilestonesAvailable
		{
			get { return _milestonesAvailable; }
            private set { this.RaiseAndSetIfChanged(ref _milestonesAvailable, value); }
		}

		public UserModel AssignedTo
		{
			get { return _assignedTo; }
            set { this.RaiseAndSetIfChanged(ref _assignedTo, value); }
		}

		private string _version;
		public string Version
		{
			get { return _version; }
            set { this.RaiseAndSetIfChanged(ref _version, value); }
		}

		private bool _versionsAvailable;
		public bool VersionsAvailable
		{
			get { return _versionsAvailable; }
            private set { this.RaiseAndSetIfChanged(ref _versionsAvailable, value); }
		}

		private string _component;
		public string Component
		{
			get { return _component; }
            set { this.RaiseAndSetIfChanged(ref _component, value); }
		}

		private bool _componentsAvailable;
		public bool ComponentsAvailable
		{
			get { return _componentsAvailable; }
            private set { this.RaiseAndSetIfChanged(ref _componentsAvailable, value); }
		}

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

        protected IssueModifyViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Kind = "bug";
            Priority = "major";

            SaveCommand = ReactiveCommand.CreateAsyncTask(t => Save());

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
            });
        }

		protected Task Load()
		{
			return Task.FromResult(false);
//			Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.GetMilestones(
//
//
//                try
//                {
//                    if (Milestones == null)
//                        Milestones = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetMilestones();
//                }
//                catch (Exception)
//                {
//                }
//
//                try
//                {
//                    if (Components == null)
//                        Components = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetComponents();
//                }
//                catch (Exception)
//                {
//                }
//
//                try
//                {
//                    if (Versions == null)
//                        Versions = Application.Client.Users[Username].Repositories[RepoSlug].Issues.GetVersions();
//                }
//                catch (Exception)
//                {
//                }
		}

		protected abstract Task Save();
    }
}

