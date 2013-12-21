using System;
using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueAssignedToViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<UserModel> _users = new CollectionViewModel<UserModel>();

		private UserModel _selectedUser;

		public UserModel SelectedUser
		{
			get
			{
				return _selectedUser;
			}
			set
			{
				_selectedUser = value;
				RaisePropertyChanged(() => SelectedUser);
			}
		}

		public CollectionViewModel<UserModel> Users
		{
			get { return _users; }
		}

		public string Username 
		{ 
			get; 
			private set; 
		}

		public string Repository 
		{ 
			get; 
			private set; 
		}

		public void Init(NavObject navObject) 
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
			SelectedUser = TxSevice.Get() as UserModel;
			this.Bind(x => x.SelectedUser, x => {
				Messenger.Publish(new SelectedAssignedToMessage(this) { User = x });
				ChangePresentation(new Cirrious.MvvmCross.ViewModels.MvxClosePresentationHint(this));
			});
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
			return Task.Run(() =>
			{
					var priv = this.GetApplication().Client.Users[Username].Repositories[Repository].Privileges.GetPrivileges(forceCacheInvalidation);
			});
//
//			List<PrivilegeModel> privileges;
//			try
//			{
//				if (Repo != null)
//				{
//					privileges = new List<PrivilegeModel>();
//					privileges.AddRange(Application.Client.Users[User].Repositories[Repo].Privileges.GetPrivileges(force));
//
//					//Get it from the group
//					try
//					{
//						var groupPrivileges = Application.Client.Users[User].Repositories[Repo].GroupPrivileges.GetPrivileges(force);
//						groupPrivileges.ForEach(x => {
//							if (x.Group == null || x.Group.Members == null)
//								return;
//
//							x.Group.Members.ForEach(m => {
//								if (!privileges.Any(p => p.User.Equals(m)))
//									privileges.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
//							});
//						});
//					}
//					catch (Exception e)
//					{
//						MonoTouch.Utilities.LogException("Unable to get privileges from group", e);
//					}
//				}
//				else
//				{
//					privileges = Application.Client.Users[User].Privileges.GetPrivileges(force);
//				}
//			}
//			catch (Exception)
//			{
//				privileges = new List<PrivilegeModel>();
//			}
//
//			Model = new ListModel<PrivilegeModel> {
//				Data = privileges
//			};
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
    }
}

