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

        private IssueModel _updateIssue;
        private int _firstIndex;
        private int _lastIndex;
        private LoadMoreElement _loadMore;

        //The filter for this view
        private FilterModel _filterModel = Application.Account.IssueFilterObject;

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
                    Success = issue => { _updateIssue = issue; }
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

        private IssuesModel OnGetData(int start = 0, int limit = 50, IEnumerable<Tuple<string, string>> additionalFilters = null)
        {
            LinkedList<Tuple<string, string>> filter = new LinkedList<Tuple<string, string>>();
            if (_filterModel != null)
            {
                if (_filterModel.Status != null && !_filterModel.Status.IsDefault())
                    foreach (var a in FieldToUrl("status", _filterModel.Status)) filter.AddLast(a);
                if (_filterModel.Kind != null && !_filterModel.Kind.IsDefault())
                    foreach (var a in FieldToUrl("kind", _filterModel.Kind)) filter.AddLast(a);
                if (_filterModel.Priority != null && !_filterModel.Priority.IsDefault())
                    foreach (var a in FieldToUrl("priority", _filterModel.Priority)) filter.AddLast(a);
                if (!string.IsNullOrEmpty(_filterModel.AssignedTo))
                {
                    if (_filterModel.AssignedTo.Equals("unassigned"))
                        filter.AddLast(new Tuple<string, string>("responsible", ""));
                    else
                        filter.AddLast(new Tuple<string, string>("responsible", _filterModel.AssignedTo));
                }
                if (!string.IsNullOrEmpty(_filterModel.ReportedBy))
                    filter.AddLast(new Tuple<string, string>("reported_by", _filterModel.ReportedBy));

                filter.AddLast(new Tuple<string, string>("sort", ((FilterModel.Order)_filterModel.OrderBy).ToString().ToLower()));
                Console.WriteLine("Sorting: " + _filterModel.OrderBy.ToString());
            }

            if (additionalFilters != null)
                foreach (var f in additionalFilters)
                    filter.AddLast(f);

           return Application.Client.Users[User].Repositories[Slug].Issues.GetIssues(start, limit, filter);
        }

        private bool DoesIssueBelong(IssueModel model)
        {
            if (_filterModel == null)
                return true;

            if (_filterModel.Status != null && !_filterModel.Status.IsDefault())
                if (!FieldToUrl(null, _filterModel.Status).Any(x => x.Item2.Equals(model.Status)))
                    return false;
            if (_filterModel.Kind != null && !_filterModel.Kind.IsDefault())
                if (!FieldToUrl(null, _filterModel.Kind).Any(x => x.Item2.Equals(model.Metadata.Kind)))
                    return false;
            if (_filterModel.Priority != null && !_filterModel.Priority.IsDefault())
                if (!FieldToUrl(null, _filterModel.Priority).Any(x => x.Item2.Equals(model.Priority)))
                    return false;


            return true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_updateIssue != null)
            {
                if (DoesIssueBelong(_updateIssue))
                {
                    AddItems(new List<IssueModel>(1) { _updateIssue }, true);
                    TableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Middle, true);
                }
                _updateIssue = null;
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
                AddItems(newEvents);

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

        private void AddItems(List<IssueModel> issues, bool prepend = false)
        {
            if (issues.Count == 0)
                return;

            Section sec;
            if (Root != null && Root.Count > 0)
                sec = Root[0];
            else
                sec = new Section();

            int inserts = 0;
            issues.ForEach(x => {
                if (sec.Elements.Any(y => ((IssueElement)y).Model.LocalId == x.LocalId))
                    return;
                if (prepend)
                    sec.Insert(0, CreateElement(x));
                else
                    sec.Add(CreateElement(x));
                inserts++;
            });

            if (sec.Count == 0 || inserts == 0)
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
            });
        }

        protected override IssuesModel OnUpdate(bool forced)
        {
            //forced doesnt matter. Never cached.
            //Update everything we have here!
            var currentCount = OnGetData(0, 0).Count;
            var moreEvents = OnGetData();
            _firstIndex = currentCount;
            _lastIndex = moreEvents.Issues.Count;
            return moreEvents;
        }

        private void ApplyFilter()
        {
            _firstIndex = _lastIndex = 0;
            Model = null;
            Root.Clear();
            Refresh();
        }

        protected override FilterController CreateFilterController()
        {
            return new Filter(this);
        }

        public class FilterModel
        {
            public string AssignedTo { get; set; }
            public string ReportedBy { get; set; }
            public StatusModel Status { get; set; }
            public KindModel Kind { get; set; }
            public PriorityModel Priority { get; set; }
            public List<string> Components { get; set; }
            public List<string> Versions { get; set; }
            public List<string> Milestones { get; set; }
            public int OrderBy { get; set; }

            public FilterModel()
            {
                Kind = new KindModel();
                Status = new StatusModel();
                Priority = new PriorityModel();
                OrderBy = (int)Order.Local_Id;
            }

            public enum Order : int
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

            private EntryElement _assignedTo;
            private EntryElement _reportedBy;
            private MultipleChoiceElement<FilterModel.StatusModel> _statusChoice;
            private MultipleChoiceElement<FilterModel.KindModel> _kindChoice;
            private MultipleChoiceElement<FilterModel.PriorityModel> _priorityChoice;
            private EnumChoiceElement _orderby;
            
            public Filter(IssuesController parent)
            {
                _parent = parent;
            }

            private FilterModel CreateFilterModel()
            {
                var model = new FilterModel();
                model.AssignedTo = _assignedTo.Value;
                model.ReportedBy = _reportedBy.Value;
                model.Status = _statusChoice.Obj;
                model.Priority = _priorityChoice.Obj;
                model.Kind = _kindChoice.Obj;
                model.OrderBy = _orderby.Obj;
                return model;
            }

            public override void ApplyFilter()
            {
                _parent._filterModel = CreateFilterModel();
                _parent.ApplyFilter();
            }

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();
                
                //Load the root
                var root = new RootElement(Title) {
                    new Section("Filter") {
                        (_assignedTo = new InputElement("Assigned To", "Anybody", _parent._filterModel.AssignedTo) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                        (_reportedBy = new InputElement("Reported By", "Anybody", _parent._filterModel.ReportedBy) { TextAlignment = UITextAlignment.Right, AutocorrectionType = UITextAutocorrectionType.No, AutocapitalizationType = UITextAutocapitalizationType.None }),
                        (_kindChoice = CreateMultipleChoiceElement("Kind", _parent._filterModel.Kind.Clone())),
                        (_statusChoice = CreateMultipleChoiceElement("Status", _parent._filterModel.Status.Clone())),
                        (_priorityChoice = CreateMultipleChoiceElement("Priority", _parent._filterModel.Priority.Clone())),
                    },
                    new Section("Order By") {
                        (_orderby = CreateEnumElement("Field", (int)_parent._filterModel.OrderBy, typeof(FilterModel.Order))),
                    },
                    new Section() {
                        new StyledElement("Save as Default", () => {  
                            var model = CreateFilterModel();
                            Application.Account.IssueFilterObject = model;
                            this.DismissModalViewControllerAnimated(true); 
                            this.ApplyFilter();
                        }),
                    }
                };
                
                Root = root;
            }
            
            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                TableView.ReloadData();
            }
        }
    }
}

