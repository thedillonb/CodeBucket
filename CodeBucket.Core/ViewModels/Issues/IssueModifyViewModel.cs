using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
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
			set
			{
				_title = value;
				RaisePropertyChanged(() => Title);
			}
		}

		public string Content
		{
			get { return _content; }
			set
			{
				_content = value;
				RaisePropertyChanged(() => Content);
			}
		}

		private string _milestone;
		public string Milestone
		{
			get { return _milestone; }
			set
			{
				_milestone = value;
				RaisePropertyChanged(() => Milestone);
			}
		}

		private bool _milestonesAvailable;
		public bool MilestonesAvailable
		{
			get { return _milestonesAvailable; }
			private set
			{
				_milestonesAvailable = value;
				RaisePropertyChanged(() => MilestonesAvailable);
			}
		}

		public UserModel AssignedTo
		{
			get { return _assignedTo; }
			set
			{
				_assignedTo = value;
				RaisePropertyChanged(() => AssignedTo);
			}
		}

		private string _version;
		public string Version
		{
			get { return _version; }
			set
			{
				_version = value;
				RaisePropertyChanged(() => Version);
			}
		}

		private bool _versionsAvailable;
		public bool VersionsAvailable
		{
			get { return _versionsAvailable; }
			private set
			{
				_versionsAvailable = value;
				RaisePropertyChanged(() => VersionsAvailable);
			}
		}

		private string _component;
		public string Component
		{
			get { return _component; }
			set
			{
				_component = value;
				RaisePropertyChanged(() => Component);
			}
		
		}

		private bool _componentsAvailable;
		public bool ComponentsAvailable
		{
			get { return _componentsAvailable; }
			private set
			{
				_componentsAvailable = value;
				RaisePropertyChanged(() => ComponentsAvailable);
			}
		}

		private string _kind;
		public string Kind
		{
			get { return _kind; }
			set
			{
				_kind = value;
				RaisePropertyChanged(() => Kind);
			}
		}

		private string _priority;
		public string Priority
		{
			get { return _priority; }
			set
			{
				_priority = value;
				RaisePropertyChanged(() => Priority);
			}
		}
       
		public bool IsSaving
		{
			get
			{
				return _isSaving;
			}
			set
			{
				_isSaving = value;
				RaisePropertyChanged(() => IsSaving);
			}
		}

		public string Username { get; private set; }

		public string Repository { get; private set; }

		public ICommand GoToMilestonesCommand
		{
			get 
			{ 
				return new MvxCommand(() => {
					GetService<IViewModelTxService>().Add(Milestone);
					ShowViewModel<IssueMilestonesViewModel>(new IssueMilestonesViewModel.NavObject { Username = Username, Repository = Repository });
				});
			}
		}

        public ICommand GoToVersionsCommand
        {
            get 
            { 
                return new MvxCommand(() => {
                    GetService<IViewModelTxService>().Add(Version);
                    ShowViewModel<IssueVersionsViewModel>(new IssueVersionsViewModel.NavObject { Username = Username, Repository = Repository });
                });
            }
        }


        public ICommand GoToComponentsCommand
        {
            get 
            { 
                return new MvxCommand(() => {
                    GetService<IViewModelTxService>().Add(Component);
                    ShowViewModel<IssueComponentsViewModel>(new IssueVersionsViewModel.NavObject { Username = Username, Repository = Repository });
                });
            }
        }


		public ICommand GoToAssigneeCommand
		{
			get 
			{ 
				return new MvxCommand(() => {
					GetService<IViewModelTxService>().Add(AssignedTo);
					ShowViewModel<IssueAssignedToViewModel>(new IssueAssignedToViewModel.NavObject { Username = Username, Repository = Repository });
				}); 
			}
		}

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

		protected override Task Load(bool forceCacheInvalidation)
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

