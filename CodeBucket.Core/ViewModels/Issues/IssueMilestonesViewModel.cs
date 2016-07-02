using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using System;
using CodeBucket.Client.Models;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueMilestonesViewModel : LoadableViewModel
	{
		private string _selectedMilestone;
		public string SelectedMilestone
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
			var issue = TxSevice.Get() as string;
			SelectedMilestone = issue;

            this.Bind(x => x.SelectedMilestone).Subscribe(x => SelectMilestone(x).ToBackground());
		}

		private async Task SelectMilestone(string x)
		{
			if (SaveOnSelect)
			{
				try
				{
					IsSaving = true;
                    var newIssue = await this.GetApplication().Client.Issues.UpdateMilestone(Username, Repository, Id, x);
					Messenger.Publish(new IssueEditMessage(this) { Issue = newIssue });
				}
				catch (Exception e)
				{
                    DisplayAlert("Unable to update issue milestone: " + e.Message).ToBackground();
				}
				finally
				{
					IsSaving = false;
				}
			}
			else
			{
                Messenger.Publish(new SelectedMilestoneMessage(this) { Value = x });
			}

			ChangePresentation(new MvxClosePresentationHint(this));
		}

		protected override async Task Load()
		{
            var items = await this.GetApplication().Client.Issues.GetMilestones(Username, Repository);
            Milestones.Items.Reset(items);
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

