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
using CodeBucket.Filters.Models;

namespace CodeBucket.Bitbucket.Controllers.Issues
{
    public class IssuesController : BaseModelDrivenController
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        private int _totalCount;
        private int _lastIndex;
        private readonly LoadMoreElement _loadMore;
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;
        private IssuesFilterModel _filterModel;

        //The filter for this view
        private IssuesFilterModel FilterModel
        {
            get { return _filterModel; }
            set 
            {
                _filterModel = value;
                Application.Account.AddFilter(this, value);
            }
        }

        public IssuesController(string user, string slug)
        {
            User = user;
            Slug = slug;
            Style = UITableViewStyle.Plain;
            EnableSearch = true;
            EnableFilter = true;
            Root.UnevenRows = true;
            Title = "Issues".t();
            SearchPlaceholder = "Search Issues".t();

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Add, () => {
                var b = new IssueEditController {
                    Username = User,
                    RepoSlug = Slug,
                    Success = OnCreateIssue
                };
                NavigationController.PushViewController(b, true);
            }));

            _loadMore = new PaginateElement("Load More".t(), "Loading...".t(), e => GetMore()) { AutoLoadOnVisible = true };
            _viewSegment = new UISegmentedControl(new string[] { "All".t(), "Open".t(), "Mine".t(), "Custom".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);

            var filter = Application.Account.GetFilter(this.GetType().Name);
            _filterModel = filter != null ? filter.GetData<IssuesFilterModel>() : new IssuesFilterModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            BeginInvokeOnMainThread(delegate {
                _viewSegment.SelectedSegment = 1;
                _viewSegment.SelectedSegment = 0;

                //Select which one is currently selected
                if (_filterModel.Equals(IssuesFilterModel.CreateAllFilter()))
                    _viewSegment.SelectedSegment = 0;
                else if (_filterModel.Equals(IssuesFilterModel.CreateOpenFilter()))
                    _viewSegment.SelectedSegment = 1;
                else if (_filterModel.Equals(IssuesFilterModel.CreateMineFilter(Application.Account.Username)))
                    _viewSegment.SelectedSegment = 2;
                else
                    _viewSegment.SelectedSegment = 3;
                    
                _viewSegment.ValueChanged += (sender, e) => SegmentValueChanged();
            });

            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        private void SegmentValueChanged()
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ApplyFilter(IssuesFilterModel.CreateAllFilter());
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ApplyFilter(IssuesFilterModel.CreateOpenFilter());
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                ApplyFilter(IssuesFilterModel.CreateMineFilter(Application.Account.Username));
            }
            else if (_viewSegment.SelectedSegment == 3)
            {

            }
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

                filter.AddLast(new Tuple<string, string>("sort", ((IssuesFilterModel.Order)_filterModel.OrderBy).ToString().ToLower()));
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

                Render();
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
                    Render();
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

        static int[] _ceilings = CodeFramework.Filters.Controllers.FilterController.IntegerCeilings;
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

        protected override void OnRender()
        {
            var order = (IssuesFilterModel.Order)_filterModel.OrderBy;
            List<Section> sections = null;
            var model = Model as List<IssueModel>;

            if (order == IssuesFilterModel.Order.Status)
            {
                var a = model.GroupBy(x => x.Status);
                sections = CreateSection(a);
            }
            else if (order == IssuesFilterModel.Order.Priority)
            {
                var a = model.GroupBy(x => x.Priority);
                sections = CreateSection(a);
            }
            else if (order == IssuesFilterModel.Order.Utc_Last_Updated)
            {
                var a = model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => _ceilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                sections = CreateSection(a, "Days Ago", "Updated");
            }
            else if (order == IssuesFilterModel.Order.Created_On)
            {
                var a = model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => _ceilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                sections = CreateSection(a, "Days Ago", "Created");
            }
            else if (order == IssuesFilterModel.Order.Version)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Version)) ? x.Metadata.Version : "No Version");
                sections = CreateSection(a);
            }
            else if (order == IssuesFilterModel.Order.Component)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Component)) ? x.Metadata.Component : "No Component");
                sections = CreateSection(a);
            }
            else if (order == IssuesFilterModel.Order.Milestone)
            {
                var a = model.GroupBy(x => (x.Metadata != null && !string.IsNullOrEmpty(x.Metadata.Milestone)) ? x.Metadata.Milestone : "No Milestone");
                sections = CreateSection(a);
            }
            else
            {
                IEnumerable<IssueModel> a;
                if (order == IssuesFilterModel.Order.Local_Id)
                    a = model.OrderBy(x => x.LocalId);
                else
                    a = model.OrderBy(x => x.Title);
                sections = new List<Section>() { new Section() };
                foreach (var y in a)
                    sections[0].Add(CreateElement(y));
            }


            if (sections == null || sections.Count == 0)
                sections.Add(new Section() { new NoItemsElement("No Issues") });


            var root = new RootElement(Title);
            root.UnevenRows = true;
            
            //If there are more items to load then insert the load object
            if (_lastIndex != _totalCount)
                sections.Add(new Section { _loadMore });

            root.Add(sections);
            Root = root;
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
            Render();
        }

        protected override object OnUpdateModel(bool forced)
        {
            //forced doesnt matter. Never cached.
            //Update everything we have here!
            var totalCount = OnGetData(0, 0).Count;
            var moreEvents = OnGetData();
            _totalCount = totalCount;
            _lastIndex = moreEvents.Issues.Count;
            return moreEvents.Issues;
        }

        private void ApplyFilter(IssuesFilterModel filterModel)
        {
            FilterModel = filterModel;
            _totalCount = _lastIndex = 0;
            Model = null;
            Root.Clear();
            UpdateAndRender();
        }
    }
}

