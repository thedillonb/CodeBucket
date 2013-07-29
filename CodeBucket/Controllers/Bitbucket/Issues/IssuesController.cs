using System;
using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch;
using CodeBucket.Bitbucket.Controllers.Privileges;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Issues
{
    public class IssuesController : Controller
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        private int _totalCount;
        private int _lastIndex;
        private LoadMoreElement _loadMore;
        private TitleView _titleView;

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
            Root.UnevenRows = true;
            SearchPlaceholder = "Search Issues";

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Add, () => {
                var b = new IssueEditController {
                    Username = User,
                    RepoSlug = Slug,
                    Success = OnCreateIssue
                };
                NavigationController.PushViewController(b, true);
            }));

            _titleView = new TitleView();
            RefreshCaption();
            NavigationItem.TitleView = _titleView;

            _loadMore = new PaginateElement("Load More", "Loading...", e => GetMore());
        }

        private void RefreshCaption()
        {
            _titleView.SetCaption("Issues", _filterModel.IsFiltering() ? "Filter Enabled" : null);
        }

        private void OnCreateIssue(IssueModel issue)
        {
            if (!DoesIssueBelong(issue))
                return;

            AddItems(new List<IssueModel>() { issue });
            ScrollToModel(issue);
        }

        private IssueElement CreateElement(IssueModel model)
        {
            var assigned = model.Responsible != null ? model.Responsible.Username : "unassigned";
            var kind = model.Metadata.Kind;
            if (kind.ToLower().Equals("enhancement")) 
                kind = "enhance";

            var el = new IssueElement(model.LocalId.ToString(), model.Title, assigned, model.Status, model.Priority, kind, model.UtcLastUpdated);
            el.Tag = model;
            el.Tapped += () =>
            {
                //Make sure the first responder is gone.
                View.EndEditing(true);
                var info = new IssueInfoController(User, Slug, model.LocalId) { ModelChanged = newModel => ChildChangedModel(newModel, model) };
                NavigationController.PushViewController(info, true);
            };
            return el;
        }

        private IEnumerable<Tuple<string, string>> FieldToUrl(string name, object o)
        {
            var ret = new LinkedList<Tuple<string, string>>();
            foreach (var f in o.GetType().GetFields())
            {
                if ((bool)f.GetValue(o))
                {
                    //Special case for "on hold"
                    var objectName = f.Name.ToLower();
                    if (objectName.Equals("onhold"))
                        objectName = "on hold";
                    ret.AddLast(new Tuple<string, string>(name, objectName));
                }
            }
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
            if (!string.IsNullOrEmpty(_filterModel.AssignedTo))
                if (!object.Equals(_filterModel.AssignedTo, (model.Responsible == null ? "unassigned" : model.Responsible.Username)))
                    return false;
            if (!string.IsNullOrEmpty(_filterModel.ReportedBy))
                if (model.ReportedBy == null || !object.Equals(_filterModel.ReportedBy, model.ReportedBy.Username))
                    return false;

            return true;
        }

        private void ScrollToModel(IssueModel issue, bool animate = false)
        {
            int s, r = 0;
            bool done = false;
            for (s = 0; s < Root.Count; s++)
            {
                for (r = 0; r < Root[s].Count; r++)
                {
                    var el = Root[s][r] as IssueElement;
                    if (el != null && ((IssueModel)el.Tag).LocalId == issue.LocalId)
                    {
                        done = true;
                        break;
                    }
                }
                if (done)
                    break;
            }
            
            try 
            {
                TableView.ScrollToRow(NSIndexPath.FromRowSection(r, s), UITableViewScrollPosition.Top, animate);
            }
            catch { }
        }

        private void ChildChangedModel(IssueModel changedModel, IssueModel oldModel)
        {
            //If null then it's been deleted!
            if (changedModel == null)
            {
                var c = TableView.ContentOffset;
                var m = Model as List<IssueModel>;
                m.RemoveAll(a => a.LocalId == oldModel.LocalId);
                Refresh(false);
                TableView.ContentOffset = c;
            }
            else
            {
                if (DoesIssueBelong(changedModel))
                {
                    AddItems(new List<IssueModel>(1) { changedModel });
                    ScrollToModel(oldModel);
                }
                else
                {
                    var c = TableView.ContentOffset;
                    var m = Model as List<IssueModel>;
                    m.RemoveAll(a => a.LocalId == changedModel.LocalId);
                    Refresh(false);
                    TableView.ContentOffset = c;
                }
            }
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() =>
            {
                var totalCount = OnGetData(0, 0).Count;
                var moreEvents = OnGetData(totalCount - _totalCount + _lastIndex);
                _totalCount = totalCount;
                _lastIndex += moreEvents.Issues.Count;
                var newEvents = (from s in moreEvents.Issues select s).ToList();
                if (newEvents.Count == 0)
                    return;

                InvokeOnMainThread(() => {
                    var t = TableView.ContentOffset;
                    AddItems(newEvents);
                    TableView.ContentOffset = t;
                });
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

        static int[] _ceilings = FilterController.IntegerCeilings;
        private static string CreateRangeString(int key, IEnumerable<int> ranges)
        {
            return ranges.LastOrDefault(x => x < key) + " to " + (key - 1);
        }
        
        private List<Section> CreateSection(IEnumerable<IGrouping<int, IssueModel>> results, string title, string prefix = null)
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

        private List<Section> CreateSection(IEnumerable<IGrouping<string, IssueModel>> results)
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
            InvokeOnMainThread(() => Root.Clear());

            var order = (FilterModel.Order)_filterModel.OrderBy;
            List<Section> sections = null;
            var model = Model as List<IssueModel>;

            if (order == FilterModel.Order.Status)
            {
                var a = model.GroupBy(x => x.Status);
                sections = CreateSection(a);
            }
            else if (order == FilterModel.Order.Priority)
            {
                var a = model.GroupBy(x => x.Priority);
                sections = CreateSection(a);
            }
            else if (order == FilterModel.Order.Utc_Last_Updated)
            {
                var a = model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => _ceilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                sections = CreateSection(a, "Days Ago", "Updated");
            }
            else if (order == FilterModel.Order.Created_On)
            {
                var a = model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => _ceilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                sections = CreateSection(a, "Days Ago", "Created");
            }
            else if (order == FilterModel.Order.Version)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Version)) ? x.Metadata.Version : "No Version");
                sections = CreateSection(a);
            }
            else if (order == FilterModel.Order.Component)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Component)) ? x.Metadata.Component : "No Component");
                sections = CreateSection(a);
            }
            else if (order == FilterModel.Order.Milestone)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Milestone)) ? x.Metadata.Milestone : "No Milestone");
                sections = CreateSection(a);
            }
            else
            {
                IEnumerable<IssueModel> a;
                if (order == FilterModel.Order.Local_Id)
                    a = model.OrderBy(x => x.LocalId);
                else
                    a = model.OrderBy(x => x.Title);
                sections = new List<Section>() { new Section() };
                foreach (var y in a)
                    sections[0].Add(CreateElement(y));
            }


            if (sections == null || sections.Count == 0)
                sections.Add(new Section() { new NoItemsElement("No Issues") });


            InvokeOnMainThread(() => { 
                var root = new RootElement(Title);
                root.UnevenRows = true;
                
                //If there are more items to load then insert the load object
                if (_lastIndex != _totalCount)
                    sections.Add(new Section { _loadMore });

                root.Add(sections);
                Root = root;
            });
        }

        private void AddItems(List<IssueModel> issues)
        {
            if (Model == null)
                Model = issues;
            else
            {
                //Remove any duplicates
                var model = Model as List<IssueModel>;
                model.RemoveAll(x => issues.Any(y => y.LocalId == x.LocalId));
                model.AddRange(issues);
            }

            //Refresh this 
            Refresh(false);
        }

        protected override object OnUpdate(bool forced)
        {
            //forced doesnt matter. Never cached.
            //Update everything we have here!
            var totalCount = OnGetData(0, 0).Count;
            var moreEvents = OnGetData();
            _totalCount = totalCount;
            _lastIndex = moreEvents.Issues.Count;
            return moreEvents.Issues;
        }

        private void ApplyFilter()
        {
            _totalCount = _lastIndex = 0;
            Model = null;
            RefreshCaption();
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
            public int OrderBy { get; set; }

            public FilterModel()
            {
                Kind = new KindModel();
                Status = new StatusModel();
                Priority = new PriorityModel();
                OrderBy = (int)Order.Local_Id;
            }

            public bool IsFiltering()
            {
                return !(string.IsNullOrEmpty(AssignedTo) && string.IsNullOrEmpty(ReportedBy) && Status.IsDefault() && Kind.IsDefault() && Priority.IsDefault());
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
                            this.DismissViewController(true, null); 
                            this.ApplyFilter();
                        }, Images.Size) { Accessory = UITableViewCellAccessory.None },
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

