using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using System;
using BitbucketSharp.Models;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueComponentsViewModel : LoadableViewModel
	{
        private string _selectedValue;
        public string SelectedValue
		{
			get
			{
                return _selectedValue;
			}
			set
			{
                _selectedValue = value;
                RaisePropertyChanged(() => SelectedValue);
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

        private readonly CollectionViewModel<ComponentModel> _components = new CollectionViewModel<ComponentModel>();
        public CollectionViewModel<ComponentModel> Components
		{
            get { return _components; }
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
            var value = TxSevice.Get() as string;
            SelectedValue = value;

            this.Bind(x => x.SelectedValue).Subscribe(x => SelectValue(x));
		}

        private async Task SelectValue(string x)
		{
			if (SaveOnSelect)
			{
				try
				{
					IsSaving = true;
                    var newIssue = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].UpdateComponent(x));
					Messenger.Publish(new IssueEditMessage(this) { Issue = newIssue });
				}
				catch (Exception e)
				{
                    DisplayAlert("Unable to update issue component: " + e.Message);
				}
				finally
				{
					IsSaving = false;
				}
			}
			else
			{
                Messenger.Publish(new SelectedComponentMessage(this) { Value = x });
			}

			ChangePresentation(new MvxClosePresentationHint(this));
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
            return Components.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.GetComponents(forceCacheInvalidation));
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

