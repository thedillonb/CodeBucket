using System.Threading;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CodeFramework.UI.Controllers;
using System;
using GitHubSharp.Models;

namespace CodeHub.Controllers.Repositories
{
    public class RepositoryController : Controller<List<Repository>>
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Repositories";
            Style = UITableViewStyle.Plain;
            Username = username;
            AutoHideSearch = true;
            EnableSearch = true;
            ShowOwner = true;
        } 

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(x => {
                RepositoryElement sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
                //sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
                sec.Add(sse);
            });

            //Sort them by name
            sec.Elements = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name).ToList();

            InvokeOnMainThread(delegate {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<Repository> OnUpdate()
        {
            var r = new GitHubSharp.Client("thedillonb", "Coolman18");
            return r.API.ListRepositories();
        }
    }
}