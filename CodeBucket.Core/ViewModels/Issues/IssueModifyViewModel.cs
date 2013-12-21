using System;
using CodeFramework.Core.ViewModels;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Issues
{
	public abstract class IssueModifyViewModel : BaseViewModel
    {
		private string _title;
		private string _content;
		private UserModel _assignedTo;
		private MilestoneModel _milestone;
		private MvxSubscriptionToken _labelsToken, _milestoneToken, _assignedToken;
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

		public MilestoneModel Milestone
		{
			get { return _milestone; }
			set
			{
				_milestone = value;
				RaisePropertyChanged(() => Milestone);
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

//		public ICommand GoToLabelsCommand
//		{
//			get 
//			{ 
//				return new MvxCommand(() => {
//					GetService<CodeFramework.Core.Services.IViewModelTxService>().Add(Labels);
//					ShowViewModel<IssueLabelsViewModel>(new IssueLabelsViewModel.NavObject { Username = Username, Repository = Repository });
//				}); 
//			}
//		}
//
//		public ICommand GoToMilestonesCommand
//		{
//			get 
//			{ 
//				return new MvxCommand(() => {
//					GetService<CodeFramework.Core.Services.IViewModelTxService>().Add(Milestone);
//					ShowViewModel<IssueMilestonesViewModel>(new IssueMilestonesViewModel.NavObject { Username = Username, Repository = Repository });
//				});
//			}
//		}
//
		public ICommand GoToAssigneeCommand
		{
			get 
			{ 
				return new MvxCommand(() => {
					GetService<CodeFramework.Core.Services.IViewModelTxService>().Add(AssignedTo);
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
			_milestoneToken = messenger.SubscribeOnMainThread<SelectedMilestoneMessage>(x => Milestone = x.Milestone);
			_assignedToken = messenger.SubscribeOnMainThread<SelectedAssignedToMessage>(x => AssignedTo = x.User);
		}

		protected abstract Task Save();
    }
}

