using System;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using CodeBucket.Client.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;

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
            this.Bind(x => x.SelectedUser).Subscribe(x => {
				Messenger.Publish(new SelectedAssignedToMessage(this) { User = x });
				ChangePresentation(new MvxClosePresentationHint(this));
			});
		}

		protected override async Task Load()
		{
            var owner = await this.GetApplication().Client.Users.GetUser(Username);
			if (owner.User.IsTeam)
			{
				var members = await this.GetApplication().Client.Teams.GetMembers(Username);
				var users = new List<UserModel>();
				users.AddRange(members.Values.Select(x => new UserModel { Username = x.Username, Avatar = x.Links.Avatar.Href }));
				users.Add(owner.User);
				Users.Items.Reset(users);
			}
			else
			{
				List<PrivilegeModel> privileges;
				try
				{
					privileges = new List<PrivilegeModel>();
                    privileges.AddRange(await this.GetApplication().Client.Privileges.GetRepositoryPrivileges(Username, Repository));

					//Get it from the group
					try
					{
                        var groupPrivileges = await this.GetApplication().Client.Privileges.GetRepositoryGroupPrivileges(Username, Repository);
						groupPrivileges.ForEach(x =>
						{
							if (x.Group == null || x.Group.Members == null)
								return;

							x.Group.Members.ForEach(m =>
							{
								if (!privileges.Any(p => p.User.Equals(m)))
									privileges.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
							});
						});
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("Unable to get privileges from group: {0}", e);
					}
		
				}
				catch
				{
					privileges = new List<PrivilegeModel>();
				}

				var users = privileges.Select(x => x.User).ToList();
				if (users.All(x => x.Username != Username))
					users.Add(owner.User);
				Users.Items.Reset(users);
			}
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
    }
}

