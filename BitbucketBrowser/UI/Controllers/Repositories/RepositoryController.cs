using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using System;

namespace BitbucketBrowser.UI.Controllers.Repositories
{
    public class RepositoryController : Controller<List<RepositoryDetailedModel>>
    {
        FilterModel _filterModel = Application.Account.RepoFilterObject;
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
            EnableFilter = true;
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
            OrderElements(sec);

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

        protected void OrderElements(Section sec)
        {
            var order = (FilterModel.Order)_filterModel.OrderBy;
            IEnumerable<Element> results;
            if (order == FilterModel.Order.Name)
                results = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name);
            else if (order == FilterModel.Order.Forks)
                results = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.ForkCount);
            else if (order == FilterModel.Order.LastUpdated)
                results = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.UtcLastUpdated);
            else if (order == FilterModel.Order.CreatedOn)
                results = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.UtcCreatedOn);
            else if (order == FilterModel.Order.Followers)
                results = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.FollowersCount);

            if (!_filterModel.Ascending)
                results = results.Reverse();
            sec.Elements = results.ToList();
        }

        private void ApplyFilter()
        {
            if (Root.Count == 0)
                return;

            OrderElements(Root[0]);
            Root.Reload(Root[0], UITableViewRowAnimation.Fade);
        }

        protected override FilterController CreateFilterController()
        {
            return new Filter(this);
        }

        public class FilterModel
        {
            public int OrderBy { get; set; }
            public bool Ascending { get; set; }
            
            public FilterModel()
            {
                OrderBy = (int)Order.Name;
                Ascending = true;
            }
            
            public enum Order : int
            { 
                Name, 
                [EnumDescription("Last Updated")]
                LastUpdated,
                Followers,
                Forks,
                [EnumDescription("Created Date")]
                CreatedOn, 
            };
        }

        public class Filter : FilterController
        {
            RepositoryController _parent;
            private EnumChoiceElement _orderby;
            private TrueFalseElement _ascendingElement;

            public Filter(RepositoryController parent)
            {
                _parent = parent;
            }

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();

                //Load the root
                var root = new RootElement(Title) {
                    new Section("Order By") {
                        (_orderby = CreateEnumElement("Field", (int)_parent._filterModel.OrderBy, typeof(FilterModel.Order))),
                        (_ascendingElement = new TrueFalseElement("Ascending", _parent._filterModel.Ascending)),
                    },
                    new Section() {
                        new StyledElement("Save as Default", () => {  
                            Application.Account.RepoFilterObject = CreateFilterModel();
                            this.DismissModalViewControllerAnimated(true); 
                            this.ApplyFilter();
                        }),
                    }
                };

                Root = root;
            }

            private FilterModel CreateFilterModel()
            {
                var model = new FilterModel();
                model.OrderBy = _orderby.Obj;
                model.Ascending = _ascendingElement.Value;
                return model;
            }

            public override void ApplyFilter()
            {
                _parent._filterModel = CreateFilterModel();
                _parent.ApplyFilter();
            }

            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                TableView.ReloadData();
            }
        }
    }
}