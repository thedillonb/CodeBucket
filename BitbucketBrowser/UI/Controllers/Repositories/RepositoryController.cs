using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI.Controllers.Repositories
{
    public class RepositoryController : Controller<List<RepositoryDetailedModel>>
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
            EnableFilter = true;
            ShowOwner = true;
        }

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(x =>
            {
                var sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
                sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
                sec.Add(sse);
            });

            //Sort them by name
            sec.Elements = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name).ToList();

            InvokeOnMainThread(delegate
            {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<RepositoryDetailedModel> OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].GetInfo(forced).Repositories;
        }

        protected override FilterController CreateFilterController()
        {
            return new Filter();
        }

        public class Filter : FilterController
        {
            private StyledElement _orderby;
            private bool _descending = true;
            private static string[] OrderFields = new[] { "Name", "Last Update", "Followers", "Forks" };

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();

                //Load the root
                var root = new RootElement(Title) {
                    new Section("Order By") {
                        CreateEnumElement("Field", "Name", OrderFields),
                        (_orderby = new StyledElement("Type", "Descending", UITableViewCellStyle.Value1)),
                    }
                };

                //Assign the tapped event
                _orderby.Tapped += ChangeDescendingAscending;

                Root = root;
            }

            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                TableView.ReloadData();
            }

            private void ChangeDescendingAscending()
            {
                _descending = !_descending;
                _orderby.Value = _descending ? "Descending" : "Ascending";
                Root.Reload(_orderby, UITableViewRowAnimation.None);
            }
        }
    }
}