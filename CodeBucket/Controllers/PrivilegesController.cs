using System;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System.Collections.Generic;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.Controllers
{
    public class PrivilegesController : ListController<PrivilegeModel>
    {
        public event Action<UserModel> SelectedItem;

        public string User { get; set; }

        public string Repo { get; set; }

        public UserModel Primary { get; set; }

        protected void OnSelectedItem(UserModel model)
        {
            var handler = SelectedItem;
            if (handler != null)
                handler(model);
        }

        public PrivilegesController(IView<ListModel<PrivilegeModel>> view, string user, string repo, UserModel primary)
            : base(view)
        {
            User = user;
            Repo = repo;
            Primary = primary;
        }

        public override void Update(bool force)
        {
            List<PrivilegeModel> privileges;
            try
            {
                if (Repo != null)
                {
                    privileges = new List<PrivilegeModel>();
                    privileges.AddRange(Application.Client.Users[User].Repositories[Repo].Privileges.GetPrivileges(force));

                    //Get it from the group
                    try
                    {
                        var groupPrivileges = Application.Client.Users[User].Repositories[Repo].GroupPrivileges.GetPrivileges(force);
                        groupPrivileges.ForEach(x => {
                            if (x.Group == null || x.Group.Members == null)
                                return;

                            x.Group.Members.ForEach(m => {
                                if (!privileges.Any(p => p.User.Equals(m)))
                                    privileges.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
                            });
                        });
                    }
                    catch (Exception e)
                    {
                        MonoTouch.Utilities.LogException("Unable to get privileges from group", e);
                    }
                }
                else
                {
                    privileges = Application.Client.Users[User].Privileges.GetPrivileges(force);
                }
            }
            catch (Exception)
            {
                privileges = new List<PrivilegeModel>();
            }

            Model = new ListModel<PrivilegeModel> {
                Data = privileges
            };
        }
    }
}

