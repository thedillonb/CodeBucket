using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Issues
{
    public abstract class IssueModifyViewModel : BaseViewModel
    {
        private string _title;
        public string IssueTitle
		{
			get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
		}

        private string _content;
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

        public IReactiveCommand<object> GoToMilestonesCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToVersionsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToComponentsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToAssigneeCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<Unit> DiscardCommand { get; }

        public IReactiveCommand<Unit> SaveCommand { get; }

        public string Username { get; }

        public string Repository { get; }

        protected IssueModifyViewModel(string username, string repository)
        {
            var alertDialogService = Locator.Current.GetService<IAlertDialogService>();

            Username = username;
            Repository = repository;
            Kind = "bug";
            Priority = "major";

            Milestones = new IssueMilestonesViewModel(username, repository);
            Versions = new IssueVersionsViewModel(username, repository);
            Components = new IssueComponentsViewModel(username, repository);
            Assignee = new IssueAssigneeViewModel(username, repository); 

            SaveCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.IssueTitle).Select(y => !string.IsNullOrEmpty(y)),
                t => Save());

            SaveCommand.InvokeCommand(DismissCommand);

            DiscardCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
                if (Content?.Length > 0 || IssueTitle?.Length > 0)
                {
                    var result = await alertDialogService.PromptYesNo(
                        "Discard Changes", "Are you sure you want to discard your changes?");
                    if (!result)
                        return;
                }

                DismissCommand.ExecuteIfCan();
            });
        }

		protected abstract Task Save();
    }
}

