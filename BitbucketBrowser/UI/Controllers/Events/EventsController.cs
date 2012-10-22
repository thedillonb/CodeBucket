using System;
using System.Linq;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using BitbucketBrowser.UI.Controllers.Wikis;
using BitbucketBrowser.UI.Controllers.Repositories;
using BitbucketBrowser.UI.Controllers.Issues;
using BitbucketBrowser.UI.Controllers.Changesets;
using MonoTouch;
using CodeFramework.UI.Views;

namespace BitbucketBrowser.UI.Controllers.Events
{
    public class EventsController : Controller<List<EventModel>>
    {
        private DateTime _lastUpdate = DateTime.MinValue;
        private int _firstIndex;
        private int _lastIndex;
        private LoadMoreElement _loadMore;

        public string Username { get; private set; }

        public bool ReportRepository { get; set; }

        public EventsController(string username, bool push = true)
            : base(push, true)
        {
            Title = "Events";
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            ReportRepository = false;
        }

        protected virtual EventsModel OnGetData(int start = 0, int limit = 30)
        {
            return Application.Client.Users[Username].GetEvents(start, limit);
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() =>
            {
                var currentCount = OnGetData(0, 0).Count;
                var moreEvents = OnGetData(currentCount - _firstIndex + _lastIndex);
                _firstIndex = currentCount;
                _lastIndex += moreEvents.Events.Count;
                var newEvents = (from s in moreEvents.Events
                                 orderby DateTime.Parse(s.UtcCreatedOn) descending
                                 select s).ToList();
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
            ex => Utilities.ShowAlert("Failure to load!", "Unable to load additional enries because the following error: " + ex.Message),
            () =>
            {
                if (_loadMore != null)
                    _loadMore.Animating = false;
            });
        }

        protected override List<EventModel> OnUpdate(bool forced)
        {
            var events = OnGetData();
            _firstIndex = events.Count;
            _lastIndex = events.Events.Count;

            var newEvents =
                (from s in events.Events
                 where DateTime.Parse(s.UtcCreatedOn) > _lastUpdate
                 orderby DateTime.Parse(s.UtcCreatedOn) descending
                 select s).ToList();
            if (newEvents.Count > 0)
                _lastUpdate = (from r in newEvents select DateTime.Parse(r.UtcCreatedOn)).Max();
            return newEvents;
        }

        protected override void OnRefresh()
        {
            AddItems(Model);
        }

        private void RepoTapped(EventModel e)
        {
            if (e.Repository != null)
            {
                NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
            }
        }

        private InteractiveTextView.TextBlock[] CreateDescription(EventModel eventModel, out UIImage img)
        {
            var desc = string.IsNullOrEmpty(eventModel.Description) ? "" : eventModel.Description.Replace("\n", " ").Trim();
            img = Images.Priority;

            //Drop the image
            if (eventModel.Event == EventModel.Type.Commit)
            {
                img = Images.Plus;
                if (ReportRepository)
                {
                    string repoName;
                    RepoName(eventModel, out repoName);

                    var fuckyou = UIFont.BoldSystemFontOfSize(12f);
                    return new[] { 
                        new InteractiveTextView.TextBlock("Commit to "),
                        new InteractiveTextView.TextBlock(repoName, fuckyou, UIColor.FromRGB(0, 64, 128), () => RepoTapped(eventModel)),
                        new InteractiveTextView.TextBlock(": " + desc)
                    };
                }
                return new[] { new InteractiveTextView.TextBlock("Commited: " + desc) };
            }

            return null;
            /*
            else if (eventModel.Event == EventModel.Type.CreateRepo)
            {
                img = Images.Create;
                if (ReportRepository)
                    desc = "Created Repo: " + repoName();
                else
                    desc = "Repository Created";
            }
            else if (eventModel.Event == EventModel.Type.WikiUpdated)
            {
                img = Images.Pencil;
                desc = "Updated the wiki page: " + desc;
            }
            else if (eventModel.Event == EventModel.Type.WikiCreated)
            {
                img = Images.Pencil;
                desc = "Created the wiki page: " + desc;
            }
            else if (eventModel.Event == EventModel.Type.StartFollowUser)
            {
                img = Images.HeartAdd;
                desc = "Started following a user";
            }
            else if (eventModel.Event == EventModel.Type.StartFollowRepo)
            {
                img = Images.HeartAdd;
                desc = "Started following: " + repoName();
            }
            else if (eventModel.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                desc = "Stopped following: " + repoName();
            }
            else if (eventModel.Event == EventModel.Type.IssueComment)
            {
                img = Images.CommentAdd;
                desc = "Issue commented on in " + repoName();
            }
            else if (eventModel.Event == EventModel.Type.IssueUpdated)
            {
                img = Images.ReportEdit;
                desc = "Issue updated in " + repoName();
            }
            else if (eventModel.Event == EventModel.Type.IssueReported)
            {
                img = Images.ReportEdit;
                desc = "Issue reported on in " + repoName();
            }
            else
                img = Images.Priority;
            */
        }

        private bool RepoName(EventModel eventModel, out string name)
        {
            if (eventModel.Repository == null)
            {
                name = "Unknown Repository";
                return false;
            }

            if (!eventModel.Repository.Owner.ToLower().Equals(Application.Account.Username.ToLower()))
                name = eventModel.Repository.Owner + "/" + eventModel.Repository.Name;
            else
                name = eventModel.Repository.Name;
            return true;
        }


        //Lists the supported events that we implemented so far...
        public static List<string> SupportedEvents = new List<string> { EventModel.Type.Commit, EventModel.Type.CreateRepo, EventModel.Type.WikiUpdated, EventModel.Type.WikiCreated,
            EventModel.Type.StartFollowRepo, EventModel.Type.StartFollowUser, EventModel.Type.StopFollowRepo, EventModel.Type.IssueComment,
            EventModel.Type.IssueUpdated, EventModel.Type.IssueReported                   
        };

        private void AddItems(List<EventModel> events, bool prepend = true)
        {
            var sec = new Section();
            events.ForEach(e =>
            {
                if (!SupportedEvents.Contains(e.Event))
                    return;

                UIImage small;
                var hello = CreateDescription(e, out small);

                //Get the user
                var username = e.User != null ? e.User.Username : null;
                var avatar = e.User != null ? e.User.Avatar : null;
                var newsEl = new NewsFeedElement(username, avatar, DateTime.Parse(e.UtcCreatedOn), hello, small);
                if (e.Event == EventModel.Type.Commit && e.Repository != null)
                {
                    newsEl.Tapped += () =>
                    {
                        if (NavigationController != null)
                            NavigationController.PushViewController(
                                new ChangesetInfoController(e.Repository.Owner, e.Repository.Slug, e.Node) { Repo = e.Repository }, true);
                    };
                }
                else if (e.Event == EventModel.Type.WikiCreated || e.Event == EventModel.Type.WikiUpdated)
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new WikiInfoController(e.Repository.Owner, e.Repository.Slug, e.Description), true);
                }
                else if (e.Event == EventModel.Type.CreateRepo || e.Event == EventModel.Type.StartFollowRepo || e.Event == EventModel.Type.StopFollowRepo)
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
                }
                else if (e.Event == EventModel.Type.IssueComment || e.Event == EventModel.Type.IssueUpdated || e.Event == EventModel.Type.IssueReported)
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new IssuesController(e.Repository.Owner, e.Repository.Slug), true);
                }

                sec.Add(newsEl);
            });

            if (sec.Count == 0)
            {
                return;
            }

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
    }
}