using System;
using CodeFramework.UI.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public class IssuesController : Controller<IssuesModel>
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        private DateTime _lastUpdate = DateTime.MinValue;
        private bool _needsUpdate;
        
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
        
        protected override void OnRefresh ()
        {
            if (Model.Issues.Count == 0)
                return;
            
            var items = new List<Element>();
            Model.Issues.ForEach(x => {
                items.Add(CreateElement(x));
            });
            
            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var sec = new Section();
                    sec.AddAll(items);
                    var v = new RootElement(Title) { sec };
                    v.UnevenRows = true;
                    Root = v;
                }
                else
                    Root[0].Insert(0, UITableViewRowAnimation.Top, items);
            });
        }
        
        protected override IssuesModel OnUpdate ()
        {
            var issues = Application.Client.Users[User].Repositories[Slug].Issues.GetIssues();
            
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

