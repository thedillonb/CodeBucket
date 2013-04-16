using System;
using System.Linq;
using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers.Changesets;
using BitbucketBrowser.GitHub.Controllers.Repositories;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;
using MonoTouch;
using CodeFramework.UI.Views;
using BitbucketBrowser.GitHub.Controllers.Gists;
using MonoTouch.Foundation;

namespace BitbucketBrowser.GitHub.Controllers.Events
{
    public class EventsController : Controller<List<EventModel>>
    {
        protected int _nextPage = 1;
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

        protected virtual List<EventModel> OnGetData(int start = 1)
        {
            var response = Application.GitHubClient.API.GetEvents(Username, start);
            if (response.Next != null)
                _nextPage = start + 1;
            else
                _nextPage = -1;
            return response.Data;
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() =>
            {
                AddItems(OnGetData(_nextPage));

                //Should never happen. Sanity check..
                if (_loadMore != null && _nextPage <= 0)
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
            InvokeOnMainThread(() => {
                Root.Clear();
                _nextPage = 1;
            });

            var events = OnGetData();
            return events;
        }

        protected override void OnRefresh()
        {
            AddItems(Model);
        }

        private IEnumerable<Element> CreateElement(EventModel eventModel)
        {
            var username = eventModel.Actor != null ? eventModel.Actor.Login : null;
            var avatar = eventModel.Actor != null ? eventModel.Actor.AvatarUrl : null;
            //var desc = string.IsNullOrEmpty(eventModel.Description) ? "" : eventModel.Description.Replace("\n", " ").Trim();
            UIImage img = Images.Priority;

            if (eventModel.PayloadObject == null)
                return null;

            if (eventModel.PayloadObject is EventModel.PushEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.PushEvent;
                img = Images.Plus;

                var elements = new List<Element>(obj.Commits.Count);
                obj.Commits.ForEach(x => {
                    var textBlocks = new List<NewsFeedElement.TextBlock>(10);
                    var desc = string.IsNullOrEmpty(x.Message) ? "" : x.Message.ToOneLine().Trim();
                    if (ReportRepository)
                    {
                        textBlocks.Add(new NewsFeedElement.TextBlock("Commited to "));
                        textBlocks.AddRange(RepoName(eventModel.Repo));
                        textBlocks.Add(new NewsFeedElement.TextBlock(": " + desc));
                    }
                    else
                    {
                        textBlocks.Add(new NewsFeedElement.TextBlock("Commited: " + desc));
                    }
                    var el = new NewsFeedElement(username, avatar, eventModel.CreatedAt, textBlocks, img);
                    elements.Add(el);
                });

                return elements;
            }
            else if (eventModel.PayloadObject is EventModel.GollumEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.GollumEvent;
                img = Images.Plus;

                var elements = new List<Element>(obj.Pages.Count);
                obj.Pages.ForEach(x => {
                    var textBlocks = new List<NewsFeedElement.TextBlock>(10);
                    if (ReportRepository)
                    {
                        textBlocks.Add(new NewsFeedElement.TextBlock(x.Action.ToTitleCase() + " wiki page "));
                        textBlocks.Add(CreateWikiBlock(x));
                        textBlocks.Add(new NewsFeedElement.TextBlock(" in "));
                        textBlocks.AddRange(RepoName(eventModel.Repo));
                    }
                    else
                    {
                        textBlocks.Add(new NewsFeedElement.TextBlock(x.Action.ToTitleCase() + " wiki page "));
                        textBlocks.Add(CreateWikiBlock(x));
                    }
                    var el = new NewsFeedElement(username, avatar, eventModel.CreatedAt, textBlocks, img);
                    elements.Add(el);
                });

                return elements;
            }


            //Create the blocks for the collowing events
            var blocks = new List<NewsFeedElement.TextBlock>(10);
            NSAction action = null;

            //These are normal cases!
            if (eventModel.PayloadObject is EventModel.CommitCommentEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.CommitCommentEvent;
                var desc = obj.Comment.Body.Replace("\n", " ").Trim();
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on commit in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                    blocks.Add(new NewsFeedElement.TextBlock(": " + desc));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on commit: " + desc));
            }
            else if (eventModel.PayloadObject is EventModel.CreateEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.CreateEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Created new " + obj.RefType + " in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Created new " + obj.RefType));
            }
            else if (eventModel.PayloadObject is EventModel.DeleteEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.DeleteEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Deleted " + obj.RefType + " in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Deleted " + obj.RefType));
            }
            else if (eventModel.PayloadObject is EventModel.DownloadEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.DownloadEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Created download in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Created download "));
            }
            else if (eventModel.PayloadObject is EventModel.FollowEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.FollowEvent;
                img = Images.Plus;
                blocks.Add(new NewsFeedElement.TextBlock("Begun following "));
                blocks.Add(CreateUserBlock(obj.Target.Login));
            }
            else if (eventModel.PayloadObject is EventModel.ForkEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.ForkEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Forked "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                    blocks.Add(new NewsFeedElement.TextBlock(" to "));
                    var repo = new EventModel.RepoModel { 
                        Id = obj.Forkee.Id, 
                        Name = obj.Forkee.FullName, 
                        Url = obj.Forkee.Url 
                    };
                    blocks.AddRange(RepoName(repo));
                }
                else
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Forked from "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
            }
            else if (eventModel.PayloadObject is EventModel.ForkApplyEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.ForkApplyEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Applied patch to fork "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Applied patch to fork"));
            }
            else if (eventModel.PayloadObject is EventModel.GistEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.GistEvent;
                img = Images.Plus;
                action = () => NavigationController.PushViewController(new GistInfoController(obj.Gist.Id), true);
                var desc = string.IsNullOrEmpty(obj.Gist.Description) ? "Gist " + obj.Gist.Id : obj.Gist.Description;
                blocks.Add(new NewsFeedElement.TextBlock(obj.Action.ToTitleCase() + " Gist: "));
                blocks.Add(new NewsFeedElement.TextBlock(desc, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), action));
            }
            else if (eventModel.PayloadObject is EventModel.IssueCommentEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.IssueCommentEvent;
                img = Images.CommentAdd;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on issue "));
                    blocks.Add(CreateIssueBlock(obj.Issue));
                    blocks.Add(new NewsFeedElement.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                    blocks.Add(new NewsFeedElement.TextBlock(": " + obj.Comment.Body));
                }
                else
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on issue"));
                    blocks.Add(CreateIssueBlock(obj.Issue));
                    blocks.Add(new NewsFeedElement.TextBlock(": " + obj.Comment.Body));
                }
            }
            else if (eventModel.PayloadObject is EventModel.IssuesEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.IssuesEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock(obj.Action.ToTitleCase() + " issue "));
                    blocks.Add(CreateIssueBlock(obj.Issue));
                    blocks.Add(new NewsFeedElement.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                {
                    blocks.Add(new NewsFeedElement.TextBlock(obj.Action.ToTitleCase() + " issue "));
                    blocks.Add(CreateIssueBlock(obj.Issue));
                }
            }
            else if (eventModel.PayloadObject is EventModel.MemberEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.MemberEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Member added to "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                    blocks.Add(new NewsFeedElement.TextBlock(": "));
                    blocks.Add(CreateUserBlock(obj.Member.Login));
                }
                else
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Member added: "));
                    blocks.Add(CreateUserBlock(obj.Member.Login));
                }
            }
            else if (eventModel.PayloadObject is EventModel.PublicEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.PublicEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Open sourced "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Open sourced this repository!"));
            }
            else if (eventModel.PayloadObject is EventModel.PullRequestEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.PullRequestEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock(obj.Action.ToTitleCase() + " pull request for "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock(obj.Action.ToTitleCase() + " pull request"));
            }
            else if (eventModel.PayloadObject is EventModel.PullRequestReviewCommentEvent)
            {
                var obj = eventModel.PayloadObject as EventModel.PullRequestReviewCommentEvent;
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on pull request in "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                    blocks.Add(new NewsFeedElement.TextBlock(": " + obj.Comment.Body));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on pull request: " + obj.Comment.Body));
            }
            else if (eventModel.PayloadObject is EventModel.WatchEvent)
            {
                img = Images.Plus;
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Begun watching: "));
                    blocks.AddRange(RepoName(eventModel.Repo));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Begun watching this repository"));
            }

            var element = new NewsFeedElement(username, avatar, eventModel.CreatedAt, blocks, img);
            if (action != null)
                element.Tapped += action;

            return new [] { element };
        }
        
        private void RepoTapped(string owner, string repo)
        {
            if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo))
                NavigationController.PushViewController(new RepositoryInfoController(owner, repo), true);
        }

        private NewsFeedElement.TextBlock CreateUserBlock(string username)
        {
            return new NewsFeedElement.TextBlock(username, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => NavigationController.PushViewController(new ProfileController(username), true));
        }

        private NewsFeedElement.TextBlock CreateIssueBlock(IssueModel issue)
        {
            return new NewsFeedElement.TextBlock(issue.Title, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => { });
        }

        private NewsFeedElement.TextBlock CreateWikiBlock(EventModel.GollumEvent.PageModel page)
        {
            return new NewsFeedElement.TextBlock(page.Title, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => { });
        } 

        private IEnumerable<NewsFeedElement.TextBlock> RepoName(EventModel.RepoModel repoModel)
        {
            //Most likely indicates a deleted repository
            if (repoModel == null)
                return new [] { new NewsFeedElement.TextBlock("Unknown Repository") };
            if (repoModel.Name == null)
                return new [] { new NewsFeedElement.TextBlock("(Deleted Repository)", UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)) };

            var repoSplit = repoModel.Name.Split('/');
            if (repoSplit.Length < 2)
            {
                return new [] { new NewsFeedElement.TextBlock(repoModel.Name, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)) };
            }

            var repoOwner = repoSplit[0];
            var repoName = repoSplit[1];
            if (!repoOwner.ToLower().Equals(Application.Account.Username.ToLower()))
            {
                return new [] {
                    new NewsFeedElement.TextBlock(repoOwner, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => NavigationController.PushViewController(new ProfileController(repoOwner), true)),
                    new NewsFeedElement.TextBlock("/", UIFont.BoldSystemFontOfSize(12f)),
                    new NewsFeedElement.TextBlock(repoName, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => RepoTapped(repoOwner, repoName)),
                };
            }

            //Just return the name
            return new [] { new NewsFeedElement.TextBlock(repoName, UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128), () => RepoTapped(repoOwner, repoName)) };
        }



        private void AddItems(List<EventModel> events)
        {
            var sec = new Section();
            events.ForEach(e =>
            {
                var a = CreateElement(e);
                if (a != null) sec.AddAll(a);

                //Get the user

//                if (e.Event == EventModel.Type.Commit && e.Repository != null)
//                {
//                    newsEl.Tapped += () =>
//                    {
//                        if (NavigationController != null)
//                            NavigationController.PushViewController(
//                                new ChangesetInfoController(e.Repository.Owner, e.Repository.Slug, e.Node) { Repo = e.Repository }, true);
//                    };
//                }
//                else if (e.Event == EventModel.Type.WikiCreated || e.Event == EventModel.Type.WikiUpdated)
//                {
//                    if (e.Repository != null)
//                        newsEl.Tapped += () => NavigationController.PushViewController(new WikiInfoController(e.Repository.Owner, e.Repository.Slug, e.Description), true);
//                }
//                else if (e.Event == EventModel.Type.CreateRepo || e.Event == EventModel.Type.StartFollowRepo || e.Event == EventModel.Type.StopFollowRepo)
//                {
//                    if (e.Repository != null)
//                        newsEl.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
//                }
//                else if (e.Event == EventModel.Type.IssueComment || e.Event == EventModel.Type.IssueUpdated || e.Event == EventModel.Type.IssueReported)
//                {
//                    if (e.Repository != null)
//                        newsEl.Tapped += () => NavigationController.PushViewController(new IssuesController(e.Repository.Owner, e.Repository.Slug), true);
//                }

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
                    if (_nextPage > 0)
                    {
                        _loadMore = new PaginateElement("Load More", "Loading...", e => GetMore());
                        r.Add(new Section { _loadMore });
                    }

                    Root = r;
                }
                else
                {
                    Root.Insert(Root.Count - 1, sec);
                }
            });
        }
    }
}