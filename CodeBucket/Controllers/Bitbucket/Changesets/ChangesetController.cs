using System;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;


namespace CodeBucket.Bitbucket.Controllers.Changesets
{
    public class ChangesetController : BaseModelDrivenController
    {
        private const int RequestLimit = 30;
        private string _lastNode;
        private LoadMoreElement _loadMore;

        public string User { get; private set; }

        public string Slug { get; private set; }

        public new List<ChangesetModel> Model { get { return (List<ChangesetModel>)base.Model; } }

        public ChangesetController(string user, string slug)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Changes".t();
            Root.UnevenRows = true;
        }

        protected virtual List<ChangesetModel> OnGetData(string startNode = null)
        {
            var data = Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets(RequestLimit, startNode);
            return data.Changesets.OrderByDescending(x => x.Utctimestamp).ToList();
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() => {
                var newChanges = OnGetData(_lastNode);

                //Always remove the first node since it should already be listed...
                if (newChanges.Count > 0)
                    newChanges.RemoveAt(0);

                //Save the last node...
                if (newChanges.Count > 0)
                {
                    AddItems(Root, newChanges);
                    _lastNode = newChanges.Last().Node;
                }

                //Should never happen. Sanity check..
                if (_loadMore != null && newChanges.Count == 0)
                {
                    InvokeOnMainThread(() => {
                        Root.Remove(_loadMore.Parent as Section);
                        _loadMore.Dispose();
                        _loadMore = null;
                    });
                }
            }, 
            ex => Utilities.ShowAlert("Failure to load!", "Unable to load additional enries! " + ex.Message),
            () => {
                if (_loadMore != null)
                    _loadMore.Animating = false;
            });
        }

        private void AddItems(RootElement root, List<ChangesetModel> changes)
        {
            var sec = new Section();
            changes.ForEach(x => {
                var desc = (x.Message ?? "").Replace("\n", " ").Trim();
                var el = new NameTimeStringElement { Name = x.Author, Time = (x.Utctimestamp.ToDaysAgo()), String = desc, Lines = 4 };
                el.Tapped += () => NavigationController.PushViewController(new ChangesetInfoController(User, Slug, x.Node), true);
                sec.Add(el);
            });

            if (sec.Count > 0)
            {
                InvokeOnMainThread(delegate {
                    root.Insert(root.Count - 1, sec);
                });
            }
        }


        protected override void OnRender()
        {
            //Create some needed elements
            var root = new RootElement(Title) { UnevenRows = true };
            _loadMore = new PaginateElement("Load More".t(), "Loading...".t(), e => GetMore()) { AutoLoadOnVisible = true };
            root.Add(new Section { _loadMore });

            //Add the items that were in the update
            AddItems(root, Model);

            //Update the UI
            Root = root;
        }

        protected override object OnUpdateModel(bool forced)
        {
            var changes = OnGetData();
            _lastNode = changes.Last().Node;
            return changes;
        }
    }
}

