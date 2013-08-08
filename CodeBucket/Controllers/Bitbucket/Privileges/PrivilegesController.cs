using System;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System.Collections.Generic;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Privileges
{
    public class PrivilegesController : BaseListModelController
    {
        public event Action<UserModel> SelectedItem;

        public string Username { get; set; }

        //Null if looking for user only privileges
        public string RepoSlug { get; set; }

        public UserModel Primary { get; set; }

        protected void OnSelectedItem(UserModel model)
        {
            var handler = SelectedItem;
            if (handler != null)
                handler(model);
        }

        public PrivilegesController()
            : base(typeof(List<PrivilegeModel>))
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Privileges";
            EnableSearch = true;
            SearchPlaceholder = "Search Users";
            NoItemsText = "No Users";
        }

        protected override Element CreateElement(object obj)
        {
            var user = ((PrivilegeModel)obj).User;
            StyledStringElement sse = new UserElement(user.Username, user.FirstName, user.LastName, user.Avatar);
            sse.Tapped += () => OnSelectedItem(user);
            return sse;
        }

        protected override object OnUpdateListModel(bool forced, int currentPage, ref int nextPage)
        {
            try
            {
                List<PrivilegeModel> privileges;
                if (RepoSlug != null)
                {
                    privileges = new List<PrivilegeModel>();
                    privileges.AddRange(Application.Client.Users[Username].Repositories[RepoSlug].Privileges.GetPrivileges(forced));

                    //Get it from the group
                    try
                    {
                        var groupPrivileges = Application.Client.Users[Username].Repositories[RepoSlug].GroupPrivileges.GetPrivileges(forced);
                        groupPrivileges.ForEach(x => {
                            if (x.Group == null || x.Group.Members == null)
                                return;

                            x.Group.Members.ForEach(m => {
                                privileges.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
                            });
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    privileges = Application.Client.Users[Username].Privileges.GetPrivileges(forced);
                }

//                if (!privileges.Exists((x) => x.User.Equals(Primary)))
//                {
//                    privileges.Add
//                }
//
                
                return privileges;
            }
            catch (Exception)
            {
                return new List<PrivilegeModel>();
            }
        }
    }
}

