using System;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System.Collections.Generic;
using BitbucketSharp.Models;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using CodeBucket.Controllers;

namespace CodeBucket.Bitbucket.Controllers.Privileges
{
    public class PrivilegesController : Controller<List<PrivilegeModel>>
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
            : base(true, true)
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Privileges";
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Users";
        }

        protected override void OnRefresh()
        {
            var sec = new Section();
            HashSet<UserModel> users = new HashSet<UserModel>();

            Model.ForEach(s => {
                if (s.User != null)
                    users.Add(s.User);
            });

            //Make sure this comes last. If the user already exists in the list, this will 
            //get rejected. The primary is only here because we KNOW this user is the owner
            //of this resource. If all else fails, we should atleast be able to assign to it.
            if (Primary != null)
                users.Add(Primary);

            foreach (var u in users.OrderBy(x => x.Username))
            {
                var user = u;
                StyledElement sse = new UserElement(user.Username, user.FirstName, user.LastName, user.Avatar);
                sse.Tapped += () => OnSelectedItem(user);
                sec.Add(sse);
            };

            if (sec.Count == 0)
                sec.Add(new NoItemsElement());

            InvokeOnMainThread(delegate
            {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<PrivilegeModel> OnUpdate(bool forced)
        {
            try
            {
                if (RepoSlug != null)
                {
                    var list = new List<PrivilegeModel>();
                    list.AddRange(Application.Client.Users[Username].Repositories[RepoSlug].Privileges.GetPrivileges(forced));

                    //Get it from the group
                    try
                    {
                        var groupPrivileges = Application.Client.Users[Username].Repositories[RepoSlug].GroupPrivileges.GetPrivileges(forced);
                        groupPrivileges.ForEach(x => {
                            if (x.Group == null || x.Group.Members == null)
                                return;

                            x.Group.Members.ForEach(m => {
                                list.Add(new PrivilegeModel { Privilege = x.Privilege, Repo = x.Repo, User = m });
                            });
                        });
                    }
                    catch (Exception)
                    {
                    }

                    return list;
                }
                else
                {
                    return Application.Client.Users[Username].Privileges.GetPrivileges(forced);
                }
            }
            catch (Exception)
            {
                return new List<PrivilegeModel>();
            }
        }
    }
}

