using System;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Issues
{
	public abstract class IssueModifyViewModel : LoadableViewModel
    {
		public static readonly string[] Priorities = { "Trivial", "Minor", "Major", "Critical", "Blocker" };
		public static readonly string[] Statuses = { "New", "Open", "Resolved", "On Hold", "Invalid", "Duplicate", "Wontfix" };
		public static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };

		private string _title;
		private string _content;
		private UserModel _assignedTo;
        private MvxSubscriptionToken _versionToken, _componentToken, _milestoneToken, _assignedToken;
		private bool _isSaving;

		public string Title
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

		public string Username { get; private set; }

		public string Repository { get; private set; }

        public ReactiveUI.IReactiveCommand<object> GoToMilestonesCommand { get; } = ReactiveUI.ReactiveCommand.Create();
        public ReactiveUI.IReactiveCommand<object> GoToVersionsCommand { get; } = ReactiveUI.ReactiveCommand.Create();
        public ReactiveUI.IReactiveCommand<object> GoToComponentsCommand { get; } = ReactiveUI.ReactiveCommand.Create();
        public ReactiveUI.IReactiveCommand<object> GoToAssigneeCommand { get; } = ReactiveUI.ReactiveCommand.Create();


		public ICommand SaveCommand
		{
			get { return new MvxCommand(() => Save()); }
		}

		protected void Init(string username, string repository)
		{
			Username = username;
			Repository = repository;

			var messenger = GetService<IMvxMessenger>();
            _milestoneToken = messenger.SubscribeOnMainThread<SelectedMilestoneMessage>(x => Milestone = x.Value);
            _versionToken = messenger.SubscribeOnMainThread<SelectedVersionMessage>(x => Version = x.Value);
            _componentToken = messenger.SubscribeOnMainThread<SelectedComponentMessage>(x => Component = x.Value);
			_assignedToken = messenger.SubscribeOnMainThread<SelectedAssignedToMessage>(x => AssignedTo = x.User);
		}

		protected override Task Load()
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

