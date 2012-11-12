using System;
using CodeFramework.UI.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch;
using CodeFramework.UI.Elements;
using BitbucketBrowser.UI.Controllers.Privileges;

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public class IssuesController : Controller<IssuesModel>
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        private DateTime _lastUpdate = DateTime.MinValue;
        private bool _needsUpdate;
        private int _firstIndex;
        private int _lastIndex;
        private LoadMoreElement _loadMore;
        private List<IssueModel> _loadedIssues = new List<IssueModel>(50);

        //The filter for this view
        private FilterModel _filterModel = new FilterModel();

        public IssuesController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = UITableViewStyle.Plain;
            Title = "Issues";
            EnableSearch = true;
            EnableFilter = true;
            AutoHideSearch = true;
            Root.UnevenRows = true;

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => {
                var b = new IssueEditController
                {
                    Username = User,
                    RepoSlug = Slug,
                    Success = issue => { _needsUpdate = true; }
                };
                NavigationController.PushViewController(b, true);
            });
        }

        private IssueElement CreateElement(IssueModel model)
        {
            var el = new IssueElement(model);
            el.Tapped += () =>
            {
                //Make sure the first responder is gone.
                View.EndEditing(true);
                var info = new IssueInfoController(User, Slug, model.LocalId) { ModelChanged = newModel => ChildChangedModel(el, newModel) };
                NavigationController.PushViewController(info, true);
            };
            return el;
        }

        private IEnumerable<Tuple<string, string>> FieldToUrl(string name, object o)
        {
            var ret = new LinkedList<Tuple<string, string>>();
            foreach (var f in o.GetType().GetFields())
                if ((bool)f.GetValue(o))
                    ret.AddLast(new Tuple<string, string>(name, f.Name.ToLower()));
            return ret;
        }

        private IssuesModel OnGetData(int start = 0, int limit = 50)
        {
            LinkedList<Tuple<string, string>> filter = null;
            if (_filterModel != null)
            {
                filter = new LinkedList<Tuple<string, string>>();
                if (_filterModel.Status != null && !_filterModel.Status.IsDefault())
                {
                    foreach (var a in FieldToUrl("status", _filterModel.Status)) filter.AddLast(a);
                }
                if (_filterModel.Kind != null && !_filterModel.Kind.IsDefault())
                {
                    foreach (var a in FieldToUrl("kind", _filterModel.Kind)) filter.AddLast(a);
                }

                filter.AddLast(new Tuple<string, string>("sort", _filterModel.OrderBy.ToString().ToLower()));
                Console.WriteLine("Sorting: " + _filterModel.OrderBy.ToString());
            }

           return Application.Client.Users[User].Repositories[Slug].Issues.GetIssues(start, limit, filter);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_needsUpdate)
            {
                //Item successfully added. We'll just refresh
                if (Root != null && Root.Count > 0 && Root[0].Count > 0)
                {
                    TableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);
                }

                Model = null;
                Refresh();
                _needsUpdate = false;
            }
        }


        private void ChildChangedModel(IssueElement element, IssueModel changedModel)
        {
            //If null then it's been deleted!
            if (changedModel == null)
            {
                Root[0].Remove(element);
                if (Root[0].Count == 0)
                {
                    TableView.TableFooterView.Hidden = true;
                }
            }
            else
            {
                element.Model = changedModel;
                Root.Reload(element, UITableViewRowAnimation.None);
            }
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() =>
            {
                var currentCount = OnGetData(0, 0).Count;
                var moreEvents = OnGetData(currentCount - _firstIndex + _lastIndex);
                _firstIndex = currentCount;
                _lastIndex += moreEvents.Issues.Count;
                var newEvents = (from s in moreEvents.Issues select s).ToList();
                AddItems(newEvents, false);

                //Should never happen. Sanity check..
                if (_loadMore != null && _firstIndex == _lastIndex)
                {
                    InvokeOnMainThread(() =>
                    {
                        Root.Remove(_loadMore.Parent as Section);
                        _loadMore.Dispose();
                        _loadMore = null;
                    });
                }
            },
            OnError, () =>
            {
                if (_loadMore != null)
                    _loadMore.Animating = false;
            });
        }

        private static void OnError(Exception ex)
        {
            Utilities.ShowAlert("Failure to load!", "Unable to load additional enries because the following error: " + ex.Message);
        }

        protected override void OnRefresh()
        {
            InvokeOnMainThread(() => Root.Clear());
            AddItems(Model.Issues);
        }

        private void AddItems(List<IssueModel> issues, bool prepend = true)
        {
            if (issues.Count == 0)
                return;

            var sec = new Section();
            issues.ForEach(x => sec.Add(CreateElement(x)));

            if (sec.Count == 0)
                return;

            InvokeOnMainThread(delegate
            {
                if (Root.Count == 0)
                {
                    var r = new RootElement(Title) { sec };

                    //If there are more items to load then insert the load object
                    if (_lastIndex != _firstIndex)
                    {
                        _loadMore = new PaginateElement("Load More", "Loading...", e => GetMore());
                        r.Add(new Section { _loadMore });
                    }

                    Root = r;
                }
                else
                {
                    if (prepend)
                        Root.Insert(0, sec);
                    else
                        Root.Insert(Root.Count - 1, sec);
                }
            });
        }

        protected override IssuesModel OnUpdate(bool forced)
        {
            //forced doesnt matter. Never cached.
            //Update everything we have here!
            return OnGetData();
        }

        private void ApplyFilter()
        {
            _firstIndex = _lastIndex = 0;
            _lastUpdate = DateTime.MinValue;
            Model = null;
            Root.Clear();
            Refresh();
        }

        protected override FilterController CreateFilterController()
        {
            return new Filter(this);
        }

        private class FilterModel
        {
            public string AssignedTo { get; set; }
            public StatusModel Status { get; set; }
            public KindModel Kind { get; set; }
            public PriorityModel Priority { get; set; }
            public Order OrderBy { get; set; }
            //public bool Ascending { get; set; }

            public FilterModel()
            {
                AssignedTo = "Anyone";
                Kind = new KindModel();
                Status = new StatusModel();
                Priority = new PriorityModel();
                OrderBy = Order.Local_Id;
                //Ascending = true;
            }

            public enum Order : byte 
            { 
                [EnumDescription("Number")]
                Local_Id, 
                Title,
                [EnumDescription("Last Updated")]
                Utc_Last_Updated, 
                [EnumDescription("Created Date")]
                Created_On, 
                Version, 
                Milestone, 
                Component, 
                Status, 
                Priority 
            };

            public class StatusModel
            {
                public StatusModel Clone()
                {
                    return (StatusModel)this.MemberwiseClone();
                }

                public bool IsDefault()
                {
                    return New && Open && Resolved && OnHold && Invalid && Duplicate && Wontfix;
                }

                public bool New = true, Open = true, Resolved = true, OnHold = true, Invalid = true, Duplicate = true, Wontfix = true;
            }

            public class KindModel
            {
                public KindModel Clone()
                {
                    return (KindModel)this.MemberwiseClone();
                }

                public bool IsDefault()
                {
                    return Bug && Enhancement && Proposal && Task;
                }

                public bool Bug = true, Enhancement = true, Proposal = true, Task = true;
            }

            public class PriorityModel
            {
                public PriorityModel Clone()
                {
                    return (PriorityModel)this.MemberwiseClone();
                }
                
                public bool IsDefault()
                {
                    return Trivial && Minor && Major && Critical && Blocker;
                }

                public bool Trivial = true, Minor = true, Major = true, Critical = true, Blocker = true;
            }
        }

        private class Filter : FilterController
        {
            private IssuesController _parent;
            //private bool _descending = true;

            private StyledElement _assignedTo;
            private MultipleChoiceElement<FilterModel.StatusModel> _statusChoice;
            private MultipleChoiceElement<FilterModel.KindModel> _kindChoice;
            private MultipleChoiceElement<FilterModel.PriorityModel> _priorityChoice;
            private EnumChoiceElement _orderby;
            
            public Filter(IssuesController parent)
            {
                _parent = parent;
            }

            public override void ApplyFilter()
            {
                var model = _parent._filterModel = new FilterModel();
                model.AssignedTo = _assignedTo.Value;
                //model.Ascending = !_descending;
                model.Status = _statusChoice.Obj;
                model.Kind = _kindChoice.Obj;
                model.OrderBy = (FilterModel.Order)_orderby.Obj;

                _parent.ApplyFilter();
            }

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();

                _assignedTo = new StyledElement("Responsible", _parent._filterModel.AssignedTo, UITableViewCellStyle.Value1)
                {
                    Accessory = UITableViewCellAccessory.DisclosureIndicator,
                };
                _assignedTo.Tapped += () =>
                {
                    var privileges = new PrivilegesController
                    {
                        Username = _parent.User,
                        RepoSlug = _parent.Slug,
                        Primary = new UserModel { Username = _parent.User },
                        Title = _assignedTo.Caption,
                    };
                    privileges.SelectedItem += obj =>
                    {
                        _assignedTo.Value = obj.User.Username;
                        NavigationController.PopViewControllerAnimated(true);
                    };
                    NavigationController.PushViewController(privileges, true);
                };
                
                //Load the root
                var root = new RootElement(Title) {
                    new Section("Filter") {
                        _assignedTo,
                        (_kindChoice = CreateMultipleChoiceElement("Kind", _parent._filterModel.Kind.Clone())),
                        (_statusChoice = CreateMultipleChoiceElement("Status", _parent._filterModel.Status.Clone())),
                        (_priorityChoice = CreateMultipleChoiceElement("Priority", _parent._filterModel.Priority.Clone())),
                    },
                    new Section("Order By") {
                        (_orderby = CreateEnumElement("Field", (int)_parent._filterModel.OrderBy, typeof(FilterModel.Order))),
                    }
                };
                
                Root = root;
            }
            
            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                TableView.ReloadData();
            }
//            
//            private void ChangeDescendingAscending()
//            {
//                _descending = !_descending;
//                _orderby.Value = _descending ? "Descending" : "Ascending";
//                Root.Reload(_orderby, UITableViewRowAnimation.None);
//            }
        }
    }
}

