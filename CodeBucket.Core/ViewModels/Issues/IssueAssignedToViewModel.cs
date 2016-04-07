using System;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueAssignedToViewModel : LoadableViewModel
    {
		private UserModel _selectedUser;
		public UserModel SelectedUser
		{
            get { return _selectedUser; }
            set { this.RaiseAndSetIfChanged(ref _selectedUser, value); }
		}

        public CollectionViewModel<UserModel> Users { get; } = new CollectionViewModel<UserModel>();

        public string Username { get; private set; }

        public string Repository { get; private set; }

		public void Init(NavObject navObject) 
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
			SelectedUser = TxSevice.Get() as UserModel;
            this.Bind(x => x.SelectedUser).Subscribe(x => {
				Messenger.Publish(new SelectedAssignedToMessage(this) { User = x });
				ChangePresentation(new MvxClosePresentationHint(this));
			});
		}

		protected override async Task Load()
		{
//			var owner = await Task.Run(() => this.GetApplication().Client.Users[Username].GetInfo(forceCacheInvalidation));
//			if (owner.User.IsTeam)
//			{
//				var members = await Task.Run(() => this.GetApplication().Client.Teams[Username].GetMembers(forceCacheInvalidation));
//				var users = new List<UserModel>();
//				users.AddRange(members.Values.Select(x => new UserModel { Username = x.Username, Avatar = x.Links.Avatar.Href }));
//				users.Add(owner.User);
//				Users.Items.Reset(users);
//			}
//			else
//			{
//				List<PrivilegeModel> privileges;
//				try
//				{
//					privileges = new List<PrivilegeModel>();
//					privileges.AddRange(await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Privileges.GetPrivileges(forceCacheInvalidation)));
//
//					//Get it from the group
//					try
//					{
//						var groupPrivileges = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].GroupPrivileges.GetPrivileges(forceCacheInvalidation));
//						groupPrivileges.ForEach(x =>
//						{
//							if (x.Group == null || x.Group.Members == null)
//								return;
//
//							x.Group.Members.ForEach(m =>
//							{
//								if (!privileges.Any(p => p.User.Equals(m)))
//									privileges.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
//							});
//						});
//					}
//					catch (Exception e)
//					{
//						System.Diagnostics.Debug.WriteLine("Unable to get privileges from group: {0}", e);
//					}
//		
//				}
//				catch (Exception e)
//				{
//					privileges = new List<PrivilegeModel>();
//				}
//
//				var users = privileges.Select(x => x.User).ToList();
//				if (users.All(x => x.Username != Username))
//					users.Add(owner.User);
//				Users.Items.Reset(users);
//			}
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
    }
}

