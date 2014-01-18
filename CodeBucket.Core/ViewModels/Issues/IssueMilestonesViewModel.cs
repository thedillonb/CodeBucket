using System.Threading.Tasks;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Messages;
using System.Linq;
using System;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueMilestonesViewModel : LoadableViewModel
	{
		private MilestoneModel _selectedMilestone;
		public MilestoneModel SelectedMilestone
		{
			get
			{
				return _selectedMilestone;
			}
			set
			{
				_selectedMilestone = value;
				RaisePropertyChanged(() => SelectedMilestone);
			}
		}

		private bool _isSaving;
		public bool IsSaving
		{
			get { return _isSaving; }
			private set {
				_isSaving = value;
				RaisePropertyChanged(() => IsSaving);
			}
		}

		private readonly CollectionViewModel<MilestoneModel> _milestones = new CollectionViewModel<MilestoneModel>();
		public CollectionViewModel<MilestoneModel> Milestones
		{
			get { return _milestones; }
		}

		public string Username  { get; private set; }

		public string Repository { get; private set; }

		public int Id { get; private set; }

		public bool SaveOnSelect { get; private set; }

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
			Id = navObject.Id;
			SaveOnSelect = navObject.SaveOnSelect;
			var issue = TxSevice.Get() as MilestoneModel;
			SelectedMilestone = issue;

			this.Bind(x => x.SelectedMilestone, x => SelectMilestone(x));
		}

		private async Task SelectMilestone(MilestoneModel x)
		{
			if (SaveOnSelect)
			{
				try
				{
					IsSaving = true;
					var newIssue = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].UpdateMilestone(x != null ? x.Name : null));
					Messenger.Publish(new IssueEditMessage(this) { Issue = newIssue });
				}
				catch (Exception e)
				{
					DisplayException(e);
				}
				finally
				{
					IsSaving = false;
				}
			}
			else
			{
				Messenger.Publish(new SelectedMilestoneMessage(this) { Milestone = x });
			}

			ChangePresentation(new Cirrious.MvvmCross.ViewModels.MvxClosePresentationHint(this));
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
			return Milestones.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.GetMilestones(forceCacheInvalidation));
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public int Id { get; set; }
			public bool SaveOnSelect { get; set; }
		}
	}
}

