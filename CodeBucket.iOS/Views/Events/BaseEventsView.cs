using System;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Events;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.Utils;
using CodeBucket.TableViewCells;
using BitbucketSharp.Models;

namespace CodeBucket.Views.Events
{
    public abstract class BaseEventsView : ViewModelCollectionDrivenDialogViewController
    {
        protected BaseEventsView()
        {
            Title = "Events";
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(NewsCellView.Nib, NewsCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            BindCollection(((BaseEventsViewModel)ViewModel).Events, CreateElement);
        }

        private static Element CreateElement(Tuple<EventModel, BaseEventsViewModel.EventBlock> e)
        {
            try
            {
                if (e.Item2 == null)
                    return null;

                var img = ChooseImage(e.Item1);

                var avatarUrl = e.Item1?.User?.Avatar;
                var avatar =  new Avatar(avatarUrl);
				var headerBlocks = new System.Collections.Generic.List<NewsFeedElement.TextBlock>();
				foreach (var h in e.Item2.Header)
				{
					Action act = null;
					var anchorBlock = h as BaseEventsViewModel.AnchorBlock;
					if (anchorBlock != null)
						act = anchorBlock.Tapped;
					headerBlocks.Add(new NewsFeedElement.TextBlock(h.Text, act));
				}

				var bodyBlocks = new System.Collections.Generic.List<NewsFeedElement.TextBlock>();
				foreach (var h in e.Item2.Body)
				{
					Action act = null;
					var anchorBlock = h as BaseEventsViewModel.AnchorBlock;
					if (anchorBlock != null)
						act = anchorBlock.Tapped;
					var block = new NewsFeedElement.TextBlock(h.Text, act);
					bodyBlocks.Add(block);
				}

                return new NewsFeedElement(avatar.ToUrl(), e.Item1.UtcCreatedOn, headerBlocks, bodyBlocks, img, e.Item2.Tapped, e.Item2.Multilined);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static UIImage ChooseImage(EventModel eventModel)
        {
			switch (eventModel.Event)
			{
				case EventModel.Type.ForkRepo:
                    return AtlassianIcon.Devtoolsrepositoryforked.ToImage();
				case EventModel.Type.CreateRepo:
                    return AtlassianIcon.Devtoolsrepository.ToImage();
				case EventModel.Type.Commit:
				case EventModel.Type.Pushed:
				case EventModel.Type.PullRequestFulfilled:
                    return AtlassianIcon.Devtoolscommit.ToImage();
				case EventModel.Type.WikiUpdated:
				case EventModel.Type.WikiCreated:
				case EventModel.Type.PullRequestUpdated:
                    return AtlassianIcon.Edit.ToImage();
                case EventModel.Type.WikiDeleted:
                case EventModel.Type.DeleteRepo:
                    return AtlassianIcon.Delete.ToImage();
				case EventModel.Type.StartFollowUser:
				case EventModel.Type.StartFollowRepo:
				case EventModel.Type.StopFollowRepo:
				case EventModel.Type.StartFollowIssue:
				case EventModel.Type.StopFollowIssue:
                    return AtlassianIcon.Star.ToImage();
				case EventModel.Type.IssueComment:
				case EventModel.Type.ChangeSetCommentCreated:
				case EventModel.Type.ChangeSetCommentDeleted:
				case EventModel.Type.ChangeSetCommentUpdated:
				case EventModel.Type.PullRequestCommentCreated:
				case EventModel.Type.PullRequestCommentUpdated:
				case EventModel.Type.PullRequestCommentDeleted:
                    return AtlassianIcon.Comment.ToImage();
				case EventModel.Type.IssueUpdated:
				case EventModel.Type.IssueReported:
                    return AtlassianIcon.Flag.ToImage();
				case EventModel.Type.ChangeSetLike:
				case EventModel.Type.PullRequestLike:
                    return AtlassianIcon.Like.ToImage();
				case EventModel.Type.PullRequestUnlike:
				case EventModel.Type.PullRequestRejected:
				case EventModel.Type.ChangeSetUnlike:
                    return AtlassianIcon.Like.ToImage();
                case EventModel.Type.PullRequestCreated:
                case EventModel.Type.PullRequestSuperseded:
                    return AtlassianIcon.Devtoolspullrequest.ToImage();
			}
            return AtlassianIcon.Info.ToImage();
        }
    }
}