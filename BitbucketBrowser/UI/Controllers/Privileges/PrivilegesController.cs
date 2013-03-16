using System;
using System.Linq;
using CodeFramework.UI.Controllers;
using System.Collections.Generic;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI.Controllers.Privileges
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
            if (Primary != null)
            {
                StyledElement primaryElement = new UserElement(Primary.Username, Primary.FirstName, Primary.LastName, Primary.Avatar);
                primaryElement.Tapped += () => OnSelectedItem(Primary);
                sec.Add(primaryElement);
            }

            HashSet<UserModel> users = new HashSet<UserModel>();
            Model.ForEach(s => {
                if (s.User != null)
                    users.Add(s.User);
            });

            foreach (var u in users)
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

