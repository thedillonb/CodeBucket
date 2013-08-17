using System;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers.Wikis;
using CodeBucket.Bitbucket.Controllers.Repositories;
using CodeBucket.Bitbucket.Controllers.Issues;
using CodeBucket.Bitbucket.Controllers.Changesets;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeBucket.Views.Accounts;

namespace CodeBucket.Bitbucket.Controllers.Events
{
    public class EventsController : BaseModelDrivenController
    {
        private DateTime _lastUpdate = DateTime.MinValue;
        private int _firstIndex;
        private int _lastIndex;
        private LoadMoreElement _loadMore;

        public string Username { get; private set; }

        public bool ReportRepository { get; set; }

        public EventsController(string username)
        {
            Title = "Events".t();
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            ReportRepository = true;
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

        protected override object OnUpdateModel(bool forced)
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

        protected override void OnRender()
        {
            AddItems(Model as List<EventModel>);

            if (Root.Count == 0)
            {
                Root = new RootElement(Title) { new Section { new NoItemsElement("No Events".t()) } };
            }
        }

        private IEnumerable<NewsFeedElement.TextBlock> CreateDescription(EventModel eventModel, out UIImage img, out Action elementAction)
        {
            var blocks = new List<NewsFeedElement.TextBlock>(10);
            var desc = string.IsNullOrEmpty(eventModel.Description) ? "" : eventModel.Description.Replace("\n", " ").Trim();
            img = Images.Priority;
            elementAction = null;

            //Drop the image
            if (eventModel.Event == EventModel.Type.Commit || eventModel.Event == EventModel.Type.Pushed)
            {
                img = Images.Plus;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new ChangesetInfoController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node) { Repo = eventModel.Repository }, true);
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock("Commited to "));
                    blocks.AddRange(RepoName(eventModel));
                    blocks.Add(new NewsFeedElement.TextBlock(": " + desc));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Commited: " + desc));
            }
            else if (eventModel.Event == EventModel.Type.CreateRepo)
            {
                img = Images.Create;
                if (ReportRepository)
                {
                    if (eventModel.Repository != null)
                        elementAction = () => NavigationController.PushViewController(new RepositoryInfoController(eventModel.Repository), true);

                    blocks.Add(new NewsFeedElement.TextBlock("Created repository "));
                    blocks.AddRange(RepoName(eventModel));
                }
                else
                    blocks.Add(new NewsFeedElement.TextBlock("Repository created"));
            }
            else if (eventModel.Event == EventModel.Type.WikiUpdated)
            {
                img = Images.Pencil;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new WikiInfoController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Description), true);
                blocks.Add(new NewsFeedElement.TextBlock("Updated wiki page "));
                blocks.Add(new NewsFeedElement.TextBlock(desc.TrimStart('/'), UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)));

                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel));
                }
            }
            else if (eventModel.Event == EventModel.Type.WikiCreated)
            {
                img = Images.Pencil;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new WikiInfoController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Description), true);
                blocks.Add(new NewsFeedElement.TextBlock("Created wiki page "));
                blocks.Add(new NewsFeedElement.TextBlock(desc.TrimStart('/'), UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)));

                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel));
                }
            }
            else if (eventModel.Event == EventModel.Type.WikiDeleted)
            {
                img = Images.BinClosed;
                blocks.Add(new NewsFeedElement.TextBlock("Deleted wiki page "));
                blocks.Add(new NewsFeedElement.TextBlock(desc.TrimStart('/'), UIFont.BoldSystemFontOfSize(12f), UIColor.FromRGB(0, 64, 128)));
                
                if (ReportRepository)
                {
                    blocks.Add(new NewsFeedElement.TextBlock(" in "));
                    blocks.AddRange(RepoName(eventModel));
                }
            }
            else if (eventModel.Event == EventModel.Type.StartFollowUser)
            {
                img = Images.HeartAdd;
                blocks.Add(new NewsFeedElement.TextBlock("Started following a user"));
            }
            else if (eventModel.Event == EventModel.Type.StartFollowRepo)
            {
                img = Images.HeartAdd;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new RepositoryInfoController(eventModel.Repository), true);
                blocks.Add(new NewsFeedElement.TextBlock("Started following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new RepositoryInfoController(eventModel.Repository), true);
                blocks.Add(new NewsFeedElement.TextBlock("Stopped following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueComment)
            {
                img = Images.CommentAdd;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue commented on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueUpdated)
            {
                img = Images.ReportEdit;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue updated in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueReported)
            {
                img = Images.ReportEdit;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue reported on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.ChangeSetCommentCreated || eventModel.Event == EventModel.Type.ChangeSetCommentDeleted || eventModel.Event == EventModel.Type.ChangeSetCommentUpdated
                     || eventModel.Event == EventModel.Type.ChangeSetLike || eventModel.Event == EventModel.Type.ChangeSetUnlike)
            {
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new ChangesetInfoController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node), true);

                if (eventModel.Event == EventModel.Type.ChangeSetCommentCreated)
                {
                    img = Images.CommentAdd;
                    blocks.Add(new NewsFeedElement.TextBlock("Commented on commit "));
                }
                else if (eventModel.Event == EventModel.Type.ChangeSetCommentDeleted)
                {
                    img = Images.BinClosed;
                    blocks.Add(new NewsFeedElement.TextBlock("Deleted a comment on commit "));
                }
                else if (eventModel.Event == EventModel.Type.ChangeSetCommentUpdated)
                {
                    img = Images.Pencil;
                    blocks.Add(new NewsFeedElement.TextBlock("Updated a comment on commit "));
                }
                else if (eventModel.Event == EventModel.Type.ChangeSetLike)
                {
                    img = Images.Accept;
                    blocks.Add(new NewsFeedElement.TextBlock("Approved commit "));
                }
                else if (eventModel.Event == EventModel.Type.ChangeSetUnlike)
                {
                    img = Images.Cancel;
                    blocks.Add(new NewsFeedElement.TextBlock("Unapproved commit "));
                }

                var nodeBlock = CommitBlock(eventModel);
                if (nodeBlock != null)
                    blocks.Add(nodeBlock);
                blocks.Add(new NewsFeedElement.TextBlock(" in "));
                blocks.AddRange(RepoName(eventModel));

            }
            else
                return null;

            return blocks;
        }

        private NewsFeedElement.TextBlock CommitBlock(EventModel e)
        {
            var node = e.Node;
            if (string.IsNullOrEmpty(node))
                return null;
            node = node.Substring(0, node.Length > 10 ? 10 : node.Length);
            return new NewsFeedElement.TextBlock(node, () => {
                NavigationController.PushViewController(new ChangesetInfoController(e.Repository.Owner, e.Repository.Slug, e.Node), true);
            });
        }
        
        private void RepoTapped(EventModel e)
        {
            if (e.Repository != null)
            {
                NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
            }
        }

        private IEnumerable<NewsFeedElement.TextBlock> RepoName(EventModel eventModel)
        {
            //Most likely indicates a deleted repository
            if (eventModel.Repository == null)
                return new [] { new NewsFeedElement.TextBlock("<Deleted Repository>", color: UIColor.Red) };

            var repoOwner = eventModel.Repository.Owner;
            var repoName = eventModel.Repository.Name;
            if (!repoOwner.ToLower().Equals(Application.Account.Username.ToLower()))
            {
                return new [] {
                    new NewsFeedElement.TextBlock(repoOwner, () => NavigationController.PushViewController(new ProfileView(repoOwner), true)),
                    new NewsFeedElement.TextBlock("/", UIFont.BoldSystemFontOfSize(12f)),
                    new NewsFeedElement.TextBlock(repoName, () => RepoTapped(eventModel)),
                };
            }

            //Just return the name
            return new [] { new NewsFeedElement.TextBlock(repoName, () => RepoTapped(eventModel)) };
        }
       
        private void AddItems(List<EventModel> events, bool prepend = true)
        {
            var sec = new Section();

			//This is a cheap hack to seperate out events that contain more than one peice of information
			var newEvents = new List<EventModel>();
			events.ForEach(x => {
				if (x.Event == EventModel.Type.Pushed)
				{
					//Break down the description
					try
					{
						var deserializer = new RestSharp.Deserializers.JsonDeserializer();
						var obj = deserializer.Deserialize<PushedEventDescriptionModel>(x.Description);
						if (obj != null)
						{
							obj.Commits.ForEach(y =>  {
								newEvents.Add(new EventModel() {
									Node = y.Hash,
									Description = y.Description,
									Repository = x.Repository,
									CreatedOn = x.CreatedOn,
									Event = x.Event,
									User = x.User,
									UtcCreatedOn = x.UtcCreatedOn,
								});

							});
						}
					}
					catch (Exception e) 
					{
						Utilities.LogException("Unable to deserialize a 'pushed' event description!", e);
					}
				}
				else
				{
					newEvents.Add(x);
				}
			});

			newEvents.ForEach(e =>
            {
                try
                {
                    UIImage small;
                    Action elementAction;
                    var hello = CreateDescription(e, out small, out elementAction);
                    if (hello == null)
                        return;

                    //Get the user
                    var username = e.User != null ? e.User.Username : null;
                    var avatar = e.User != null ? e.User.Avatar : null;
                    var newsEl = new NewsFeedElement(username, avatar, (e.UtcCreatedOn), hello, small);
                    if (elementAction != null)
                        newsEl.Tapped += () => elementAction();
                    sec.Add(newsEl);
                }
                catch (Exception ex)
                {
                    Utilities.LogException("Unable to add event", ex);
                }
            });

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
                        _loadMore = new PaginateElement("Load More".t(), "Loading...".t(), e => GetMore()) { AutoLoadOnVisible = true };
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