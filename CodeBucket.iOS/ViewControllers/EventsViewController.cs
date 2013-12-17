using System;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers.Issues;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeBucket.ViewControllers;

namespace CodeBucket.ViewControllers
{
    public class EventsViewController : BaseListControllerDrivenViewController, IListView<EventModel>
    {
        public string Username { get; private set; }

        public bool ReportRepository { get; set; }

        public EventsViewController(string username)
        {
            Title = "Events".t();
            Username = username;
            Root.UnevenRows = true;
            ReportRepository = true;
            EnableSearch = false;
            Controller = new EventsController(this, username);
        }

        public void Render(ListModel<EventModel> model)
        {
            model.Data = ExpandConsolidatedEvents(model.Data);
            RenderList(model, e => {
                try
                {
                    UIImage small;
                    Action elementAction;
                    var hello = CreateDescription(e, out small, out elementAction);
                    if (hello == null)
                        return null;

                    //Get the user
                    var username = e.User != null ? e.User.Username : null;
                    var avatar = e.User != null ? e.User.Avatar : null;
                    var newsEl = new NewsFeedElement(username, avatar, (e.UtcCreatedOn), hello, small);
                    if (elementAction != null)
                        newsEl.Tapped += () => elementAction();
                    return newsEl;
                }
                catch (Exception ex)
                {
                    Utilities.LogException("Unable to add event", ex);
                    return null;
                }
            });
        }

        private static List<EventModel> ExpandConsolidatedEvents(List<EventModel> events)
        {
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

            return newEvents;
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
                    elementAction = () => NavigationController.PushViewController(new ChangesetInfoViewController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node) { Repo = eventModel.Repository }, true);
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
                        elementAction = () => NavigationController.PushViewController(new RepositoryInfoViewController(eventModel.Repository), true);

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
                    elementAction = () => NavigationController.PushViewController(new WikiViewController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Description), true);
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
                    elementAction = () => NavigationController.PushViewController(new WikiViewController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Description), true);
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
                    elementAction = () => NavigationController.PushViewController(new RepositoryInfoViewController(eventModel.Repository), true);
                blocks.Add(new NewsFeedElement.TextBlock("Started following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new RepositoryInfoViewController(eventModel.Repository), true);
                blocks.Add(new NewsFeedElement.TextBlock("Stopped following "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueComment)
            {
                img = Images.CommentAdd;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesViewController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue commented on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueUpdated)
            {
                img = Images.ReportEdit;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesViewController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue updated in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.IssueReported)
            {
                img = Images.ReportEdit;
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new IssuesViewController(eventModel.Repository.Owner, eventModel.Repository.Slug), true);
                blocks.Add(new NewsFeedElement.TextBlock("Issue reported on in "));
                blocks.AddRange(RepoName(eventModel));
            }
            else if (eventModel.Event == EventModel.Type.ChangeSetCommentCreated || eventModel.Event == EventModel.Type.ChangeSetCommentDeleted || eventModel.Event == EventModel.Type.ChangeSetCommentUpdated
                     || eventModel.Event == EventModel.Type.ChangeSetLike || eventModel.Event == EventModel.Type.ChangeSetUnlike)
            {
                if (eventModel.Repository != null)
                    elementAction = () => NavigationController.PushViewController(new ChangesetInfoViewController(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node), true);

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
                NavigationController.PushViewController(new ChangesetInfoViewController(e.Repository.Owner, e.Repository.Slug, e.Node), true);
            });
        }
        
        private void RepoTapped(EventModel e)
        {
            if (e.Repository != null)
            {
                NavigationController.PushViewController(new RepositoryInfoViewController(e.Repository), true);
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
                    new NewsFeedElement.TextBlock(repoOwner, () => NavigationController.PushViewController(new ProfileViewController(repoOwner), true)),
                    new NewsFeedElement.TextBlock("/", UIFont.BoldSystemFontOfSize(12f)),
                    new NewsFeedElement.TextBlock(repoName, () => RepoTapped(eventModel)),
                };
            }

            //Just return the name
            return new [] { new NewsFeedElement.TextBlock(repoName, () => RepoTapped(eventModel)) };
        }
    }
}