using System;
using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Events;
using MonoTouch;
using UIKit;
using BitbucketSharp.Models;
using CodeBucket.ViewControllers;
using CodeBucket.Core.Utils;

namespace CodeBucket.Views.Events
{
    public abstract class BaseEventsView : ViewModelCollectionDrivenDialogViewController
    {
        protected BaseEventsView()
        {
            Title = "Events";
            Root.UnevenRows = true;
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			TableView.SeparatorInset = CodeBucket.Cells.NewsCellView.EdgeInsets;
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
					if (act == null) block.Color = UIColor.DarkGray;
					bodyBlocks.Add(block);
				}

                return new NewsFeedElement(avatar.ToUrl(), e.Item1.UtcCreatedOn, headerBlocks, bodyBlocks, img, e.Item2.Tapped);
            }
            catch (Exception ex)
            {
                Utilities.LogException("Unable to add event", ex);
                return null;
            }
        }

        private static UIImage ChooseImage(EventModel eventModel)
        {
			switch (eventModel.Event)
			{
				case EventModel.Type.ForkRepo:
					return Images.Fork;
				case EventModel.Type.CreateRepo:
					return Images.Repo;
				case EventModel.Type.Commit:
				case EventModel.Type.Pushed:
				case EventModel.Type.PullRequestFulfilled:
					return Images.Commit;
				case EventModel.Type.WikiUpdated:
				case EventModel.Type.WikiCreated:
				case EventModel.Type.PullRequestUpdated:
					return Images.Pencil;
				case EventModel.Type.WikiDeleted:
				case EventModel.Type.DeleteRepo:
					return Images.BinClosed;
				case EventModel.Type.StartFollowUser:
				case EventModel.Type.StartFollowRepo:
				case EventModel.Type.StopFollowRepo:
				case EventModel.Type.StartFollowIssue:
				case EventModel.Type.StopFollowIssue:
					return Images.Following;
				case EventModel.Type.IssueComment:
				case EventModel.Type.ChangeSetCommentCreated:
				case EventModel.Type.ChangeSetCommentDeleted:
				case EventModel.Type.ChangeSetCommentUpdated:
				case EventModel.Type.PullRequestCommentCreated:
				case EventModel.Type.PullRequestCommentUpdated:
				case EventModel.Type.PullRequestCommentDeleted:
					return Images.Comments;
				case EventModel.Type.IssueUpdated:
				case EventModel.Type.IssueReported:
					return Images.Flag;
				case EventModel.Type.ChangeSetLike:
				case EventModel.Type.PullRequestLike:
					return Images.Accept;
				case EventModel.Type.PullRequestUnlike:
				case EventModel.Type.PullRequestRejected:
				case EventModel.Type.ChangeSetUnlike:
					return Images.Cancel;
				case EventModel.Type.PullRequestCreated:
				case EventModel.Type.PullRequestSuperseded:
					return Images.Hand;
			}
            return Images.Priority;
        }
    }
}