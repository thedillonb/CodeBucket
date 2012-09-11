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

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public class IssuesController : Controller<IssuesModel>
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        private DateTime _lastUpdate = DateTime.MinValue;
        private bool _needsUpdate;
        private int _firstIndex = 0;
        private int _lastIndex = 0;
        private LoadMoreElement _loadMore;
        
        public IssuesController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Issues";
            EnableSearch = true;
            AutoHideSearch = true;
            Root.UnevenRows = true;
            
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => {
                var b = new IssueEditController() { Username = User, RepoSlug = Slug };
                b.Success = (issue) => {
                    _needsUpdate = true;
                };
                NavigationController.PushViewController(b, true);
            });
        }

        private IssuesModel OnGetData(int start = 0, int limit = 30)
        {
            return Application.Client.Users[User].Repositories[Slug].Issues.GetIssues(start, limit);
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

        private IssueElement CreateElement(IssueModel model)
        {
            var el = new IssueElement(model);
            el.Tapped += () => {
                //Make sure the first responder is gone.
                View.EndEditing(true);
                var info = new IssueInfoController(User, Slug, model.LocalId);
                info.ModelChanged = (newModel) => ChildChangedModel(el, newModel);
                NavigationController.PushViewController(info, true);
            };
            return el;
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
            this.DoWorkNoHud(() => {
                var currentCount = OnGetData(0, 0).Count;
                var moreEvents = OnGetData(currentCount - _firstIndex + _lastIndex);
                _firstIndex = currentCount;
                _lastIndex += moreEvents.Issues.Count;
                var newEvents = (from s in moreEvents.Issues
                                 orderby DateTime.Parse(s.UtcCreatedOn) descending
                                 select s).ToList();
                AddItems(newEvents, false);
                
                //Should never happen. Sanity check..
                if (_loadMore != null && _firstIndex == _lastIndex)
                {
                    InvokeOnMainThread(() => {
                        Root.Remove(_loadMore.Parent as Section);
                        _loadMore.Dispose();
                        _loadMore = null;
                    });
                }
            },
            (ex) => {
                Utilities.ShowAlert("Failure to load!", "Unable to load additional enries because the following error: " + ex.Message);
            },
            () => {
                if (_loadMore != null)
                    _loadMore.Animating = false;
            });
        }
        
        protected override void OnRefresh ()
        {
            AddItems(Model.Issues);
        }

        private void AddItems(List<IssueModel> issues, bool prepend = true)
        {
            if (issues.Count == 0)
                return;
            
            var sec = new Section();
            issues.ForEach(x => {
                sec.Add(CreateElement(x));
            });

            if (sec.Count == 0)
                return;
            
            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var r = new RootElement(Title) { sec };
                    
                    //If there are more items to load then insert the load object
                    if (_lastIndex != _firstIndex)
                    {
                        _loadMore = new PaginateElement("Load More", "Loading...", (e) => GetMore());
                        r.Add(new Section() { _loadMore });
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
        
        protected override IssuesModel OnUpdate ()
        {
            var issues = OnGetData();
            _firstIndex = issues.Count;
            _lastIndex = issues.Issues.Count;
            
            var newChanges =
                (from s in issues.Issues
                 where DateTime.Parse(s.UtcCreatedOn) > _lastUpdate
                 orderby DateTime.Parse(s.UtcCreatedOn) descending
                 select s).ToList();
            if (newChanges.Count > 0)
                _lastUpdate = (from r in newChanges select DateTime.Parse(r.UtcCreatedOn)).Max();
            
            issues.Issues = newChanges;
            return issues;
        }
    }
}

