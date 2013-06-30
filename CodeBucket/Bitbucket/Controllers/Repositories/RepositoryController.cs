using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System;
using System.Drawing;

using CodeBucket.Cells;
using CodeBucket.Controllers;
using CodeBucket.Views;
using CodeBucket.Elements;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryController : ListController<RepositoryDetailedModel>
    {
        FilterModel _filterModel = Application.Account.RepoFilterObject;
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true, bool refresh = true)
            : base(push, refresh)
        {
            Title = "Repositories";
            Username = username;
            ShowOwner = true;
            EnableFilter = true;
        }

        protected override List<RepositoryDetailedModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var a = Application.Client.Users[Username].GetInfo(force).Repositories;
            nextPage = -1;
            return a.OrderBy(x => x.Name).ToList();
        }

        protected override Element CreateElement(RepositoryDetailedModel x)
        {
            var sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
            sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
            return sse;
        }


        static int[] _ceilings = FilterController.IntegerCeilings;
        private static string CreateRangeString(int key, IEnumerable<int> ranges)
        {
            return ranges.LastOrDefault(x => x < key) + " to " + (key - 1);
        }

        private List<Section> CreateSection(IEnumerable<IGrouping<int, RepositoryDetailedModel>> results, string title, string prefix = null)
        {
            var sections = new List<Section>();
            InvokeOnMainThread(() => {
                foreach (var groups in results)
                {
                    var text = (prefix != null ? prefix + " " : "") + CreateRangeString(groups.Key, _ceilings) + " " + title;
                    var sec = new Section(new TableViewSectionView(text));
                    sections.Add(sec);
                    foreach (var y in groups)
                        sec.Add(CreateElement(y));
                }
            });
            return sections;
        }

        private List<Section> CreateSection(IEnumerable<IGrouping<string, RepositoryDetailedModel>> results)
        {
            var sections = new List<Section>();
            InvokeOnMainThread(() => {
                foreach (var groups in results)
                {
                    var sec = new Section(new TableViewSectionView(groups.Key));
                    sections.Add(sec);
                    foreach (var y in groups)
                        sec.Add(CreateElement(y));
                }
            });
            return sections;
        }

        protected override void OnRefresh()
        {
            if (Model == null)
                return;

            var order = (FilterModel.Order)_filterModel.OrderBy;
            List<Section> sections = null;

            if (order == FilterModel.Order.Forks)
            {
                var a = Model.OrderBy(x => x.ForkCount).GroupBy(x => _ceilings.First(r => r > x.ForkCount));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Forks");
            }
            else if (order == FilterModel.Order.LastUpdated)
            {
                var a = Model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => _ceilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Updated");
            }
            else if (order == FilterModel.Order.CreatedOn)
            {
                var a = Model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => _ceilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Created");
            }
            else if (order == FilterModel.Order.Followers)
            {
                var a = Model.OrderBy(x => x.FollowersCount).GroupBy(x => _ceilings.First(r => r > x.FollowersCount));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Followers");
            }
            else if (order == FilterModel.Order.Owner)
            {
                var a = Model.OrderBy(x => x.Name).GroupBy(x => x.Owner);
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a);
            }
            else
            {
                var a = _filterModel.Ascending ? Model.OrderBy(x => x.Name) : Model.OrderByDescending(x => x.Name);
                sections = new List<Section>() { new Section() };
                foreach (var y in a)
                    sections[0].Add(CreateElement(y));
            }

            if (sections == null || sections.Count == 0)
                sections.Add(new Section() { new NoItemsElement("No Repositories") });

            InvokeOnMainThread(() => { Root = new RootElement(Title) { sections }; });
        }

        private void ApplyFilter()
        {
            OnRefresh();
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
                Owner,
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
                        }, Images.Size) { Accessory = UITableViewCellAccessory.None },
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