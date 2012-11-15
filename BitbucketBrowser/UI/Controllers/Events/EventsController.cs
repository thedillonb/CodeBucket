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
                                 orderby (s.UtcCreatedOn) descending
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
                 where (s.UtcCreatedOn) > _lastUpdate
                 orderby (s.UtcCreatedOn) descending
                 select s).ToList();
            if (newEvents.Count > 0)
                _lastUpdate = (from r in newEvents select (r.UtcCreatedOn)).Max();
            return newEvents;
        }

        protected override void OnRefresh()
        {
            AddItems(Model);
        }

        private IEnumerable<InteractiveTextView.TextBlock> CreateDescription(EventModel eventModel, out UIImage img)
        {
            var blocks = new List<InteractiveTextView.TextBlock>(10);
            var desc = string.IsNullOrEmpty(eventModel.Description) ? "" : eventModel.Description.Replace("\n", " ").Trim();
            img = Images.Priority;

            //Drop the image
            if (eventModel.Event == EventModel.Type.Commit)
            {
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new InteractiveTextView.TextBlock("Commited to "));
                    blocks.AddRange(RepoName(eventModel));
                    blocks.Add(new InteractiveTextView.TextBlock(": " + desc));
                }
                else
                    blocks.Add(new InteractiveTextView.TextBlock("Commited: " + desc));
            }
            else if (eventModel.Event == EventModel.Type.CreateRepo)
            {
                img = Images.Create;
                if (ReportRepository)
                {
                    blocks.Add(new InteractiveTextView.TextBlock("Created repository "));
                    blocks.AddRange(RepoName(eventModel));
                }
                else
                    blocks.Add(new InteractiveTextView.TextBlock("Repository created"));
            }
            else if (eventModel.Event == EventModel.Type.WikiUpdated)
            {
                img = Images.Pencil;
                blocks.Add(new InteractiveTextView.TextBlock("Updated wiki page "));
                blocks.Add(new InteractiveTextView.TextBlock(desc, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)));

                if (ReportRepository)
                {
                    blocks.Add(new InteractiveTextView.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel));
                }
            }
            else if (eventModel.Event == EventModel.Type.WikiCreated)
            {
                img = Images.Pencil;
                blocks.Add(new InteractiveTextView.TextBlock("Created wiki page "));
                blocks.Add(new InteractiveTextView.TextBlock(desc, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)));

                if (ReportRepository)
                {
                    blocks.Add(new InteractiveTextView.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel));
                }
            }
            else if (eventModel.Event == EventModel.Type.StartFollowUser)
            {
                img = Images.HeartAdd;
                blocks.Add(new InteractiveTextView.TextBlock("Started following a user"));
            }
            else if (eventModel.Event == EventModel.Type.StartFollowRepo)
            {
                img = Images.HeartAdd;
                blocks.Add(new InteractiveTextView.TextBlock("Started following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                blocks.Add(new InteractiveTextView.TextBlock("Stopped following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueComment)
            {
                img = Images.CommentAdd;
                blocks.Add(new InteractiveTextView.TextBlock("Issue commented on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueUpdated)
            {
                img = Images.ReportEdit;
                blocks.Add(new InteractiveTextView.TextBlock("Issue updated in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueReported)
            {
                img = Images.ReportEdit;
                blocks.Add(new InteractiveTextView.TextBlock("Issue reported on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else
                img = Images.Priority;

            return blocks;
        }
        
        private void RepoTapped(EventModel e)
        {
            if (e.Repository != null)
            {
                NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
            }
        }

        private IEnumerable<InteractiveTextView.TextBlock> RepoName(EventModel eventModel)
        {
            //Most likely indicates a deleted repository
            if (eventModel.Repository == null)
                return new [] { new InteractiveTextView.TextBlock("<Not Found>") };

            var repoOwner = eventModel.Repository.Owner;
            var repoName = eventModel.Repository.Name;
            if (!repoOwner.ToLower().Equals(Application.Account.Username.ToLower()))
            {
                return new [] {
                    new InteractiveTextView.TextBlock(repoOwner, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => NavigationController.PushViewController(new ProfileController(repoOwner), true)),
                    new InteractiveTextView.TextBlock("/", UIFont.BoldSystemFontOfSize(12f)),
                    new InteractiveTextView.TextBlock(repoName, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => RepoTapped(eventModel)),
                };
            }

            //Just return the name
            return new [] { new InteractiveTextView.TextBlock(repoName, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => RepoTapped(eventModel)) };
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
                var newsEl = new NewsFeedElement(username, avatar, (e.UtcCreatedOn), hello, small);
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
                    if (e.Repository != null)
                        newsEl.Tapped += () => NavigationController.PushViewController(new WikiInfoController(e.Repository.Owner, e.Repository.Slug, e.Description), true);
                }
                else if (e.Event == EventModel.Type.CreateRepo || e.Event == EventModel.Type.StartFollowRepo || e.Event == EventModel.Type.StopFollowRepo)
                {
                    if (e.Repository != null)
                        newsEl.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
                }
                else if (e.Event == EventModel.Type.IssueComment || e.Event == EventModel.Type.IssueUpdated || e.Event == EventModel.Type.IssueReported)
                {
                    if (e.Repository != null)
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