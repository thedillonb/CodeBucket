using System;
using System.Linq;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.Core.ViewModels.Users;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.PullRequests;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Core.Utils;
using Humanizer;
using CodeBucket.Core.ViewModels.Wiki;

namespace CodeBucket.Core.ViewModels.Events
{
    public abstract class BaseEventsViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<EventItemViewModel>
    {
        int nextPage = 0;

        private bool _hasMore;
        public bool HasMore
        {
            get { return _hasMore; }
            private set { this.RaiseAndSetIfChanged(ref _hasMore, value); }
        }

        public IReadOnlyReactiveList<EventItemViewModel> Items { get; }

        public bool ReportRepository { get; private set; } = true;

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> LoadMoreCommand { get; }

        protected BaseEventsViewModel()
        {
            Title = "Events";

            var eventItems = new ReactiveList<EventItemViewModel>();
            Items = eventItems.CreateDerivedCollection(x => x);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                HasMore = false;
                nextPage = 0;
                var events = await GetEvents(nextPage, 40);
                nextPage += events.Events.Count;
                eventItems.Reset(events.Events.Select(CreateEventEventTextBlocks).Where(x => x != null));
                HasMore = nextPage < events.Count;
            });

            var hasMoreObs = this.WhenAnyValue(x => x.HasMore);
            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(hasMoreObs, async _ =>
            {
                HasMore = false;
                var events = await GetEvents(nextPage, 40);
                nextPage += events.Events.Count;
                eventItems.AddRange(events.Events.Select(CreateEventEventTextBlocks).Where(x => x != null));
                HasMore = nextPage < events.Count;
            });
        }

        protected abstract Task<EventsModel> GetEvents(int start, int limit);

		private void GoToCommits(RepositoryDetailedModel repoModel, string branch)
        {
			if (branch != null)
                NavigateTo(new CommitsViewModel(repoModel.Owner, repoModel.Name, branch));
			else
                NavigateTo(new BranchesViewModel(repoModel.Owner, repoModel.Name));
        }

		private void GoToRepository(RepositoryDetailedModel eventModel)
        {
			if (eventModel == null)
				return;
            NavigateTo(new RepositoryViewModel(eventModel.Owner, eventModel.Slug));
        }

		private void GoToRepositoryIssues(RepositoryDetailedModel eventModel)
		{
			if (eventModel == null)
				return;
            NavigateTo(new IssuesViewModel(eventModel.Owner, eventModel.Slug));
		}

        private void GoToUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return;
            NavigateTo(new UserViewModel(username));
        }

		private void GoToRepositoryWiki(RepositoryDetailedModel repository, string page)
		{
			if (repository == null)
				return;
            NavigateTo(new WikiViewModel(repository.Owner, repository.Slug, page));
		}

//
//        private void GoToBranches(RepositoryIdentifier repoId)
//        {
//			ShowViewModel<BranchesAndTagsViewModel>(new BranchesAndTagsViewModel.NavObject
//            {
//				Username = repoId.Owner,
//				Repository = repoId.Name,
//				IsShowingBranches = true
//            });
//        }
//
//        private void GoToTags(EventModel.RepoModel eventModel)
//        {
//            var repoId = new RepositoryIdentifier(eventModel.Name);
//			ShowViewModel<BranchesAndTagsViewModel>(new BranchesAndTagsViewModel.NavObject
//            {
//				Username = repoId.Owner,
//				Repository = repoId.Name,
//				IsShowingBranches = false
//            });
//        }

//        private void GoToIssue(RepositoryIdentifier repo, ulong id)
//        {
//            if (repo == null || string.IsNullOrEmpty(repo.Name) || string.IsNullOrEmpty(repo.Owner))
//                return;
//            ShowViewModel<IssueViewModel>(new IssueViewModel.NavObject
//            {
//                Username = repo.Owner,
//                Repository = repo.Name,
//                Id = id
//            });
//        }
//
//		private void GoToPullRequest(RepositoryDetailedModel repo, int id)
//        {
//			if (repo == null)
//				return;
//            ShowViewModel<PullRequestViewModel>(new PullRequestViewModel.NavObject
//            {
//                Username = repo.Owner,
//                Repository = repo.Name,
//                Id = id
//            });
//        }
//
		private void GoToPullRequests(RepositoryDetailedModel repo)
        {
			if (repo == null)
				return;
            NavigateTo(new PullRequestsViewModel(repo.Owner, repo.Slug));
        }

		private void GoToChangeset(string owner, string name, string sha)
        {
            NavigateTo(new CommitViewModel(owner, name, sha));
        }

        private EventItemViewModel CreateEventEventTextBlocks(EventModel eventModel)
        {
            var avatar = new Avatar(eventModel.User?.Avatar);
            var createdOn = eventModel.UtcCreatedOn.Humanize();
            var eventType = eventModel.Event;
            var eventBlock = new EventItemViewModel(avatar, eventType, createdOn);
			var username = eventModel.User != null ? eventModel.User.Username : null;

            // Insert the actor
			eventBlock.Header.Add(new EventAnchorBlock(username, () => GoToUser(username)));


			if (eventModel.Event == EventModel.Type.Pushed)
			{
                if (eventModel.Repository == null)
                    return null;

                var data = eventModel.Description.ToObject<PushedEventDescriptionModel>();
                var commits = data.Commits.Count;

				if (eventModel.Repository != null)
					eventBlock.Tapped = () => GoToCommits(eventModel.Repository, null);

                eventBlock.Header.Add(new EventTextBlock(" pushed " + commits + " commit" + (commits > 1 ? "s" : string.Empty)));

                if (ReportRepository)
                {
					eventBlock.Header.Add(new EventTextBlock(" to "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
                }

				if (data.Commits != null)
				{
					foreach (var commit in data.Commits)
					{
						var desc = (commit.Description ?? "");
						var sha = commit.Hash;
						var firstNewLine = desc.IndexOf("\n", StringComparison.Ordinal);
						if (firstNewLine <= 0)
							firstNewLine = desc.Length;

						desc = desc.Substring(0, firstNewLine);
						var shortSha = commit.Hash;
						if (shortSha.Length > 6)
							shortSha = shortSha.Substring(0, 6);

						eventBlock.Body.Add(new EventAnchorBlock(shortSha, () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Name, sha)));
						eventBlock.Body.Add(new EventTextBlock(" - " + desc + "\n"));
					}

                    eventBlock.Multilined = true;
				}
				return eventBlock;
			}

			if (eventModel.Event == EventModel.Type.Commit)
			{
                if (eventModel.Repository == null)
                    return null;

				var node = eventModel.Node.Substring(0, eventModel.Node.Length > 6 ? 6 : eventModel.Node.Length);
				eventBlock.Tapped = () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Name, eventModel.Node);
				eventBlock.Header.Add(new EventTextBlock(" commited "));
				eventBlock.Header.Add(new EventAnchorBlock(node, eventBlock.Tapped));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
                var description = eventModel.Description.ToObject<string>();
                var desc = string.IsNullOrEmpty(description) ? "" : description.Replace("\n", " ").Trim();
				eventBlock.Body.Add(new EventTextBlock(desc));
			}
			else if (eventModel.Event == EventModel.Type.ChangeSetCommentCreated || eventModel.Event == EventModel.Type.ChangeSetCommentDeleted ||
			         eventModel.Event == EventModel.Type.ChangeSetCommentUpdated || eventModel.Event == EventModel.Type.ChangeSetLike || eventModel.Event == EventModel.Type.ChangeSetUnlike)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node);
				var nodeBlock = CommitBlock(eventModel);

				if (eventModel.Event == EventModel.Type.ChangeSetCommentCreated)
				{
					eventBlock.Header.Add(new EventTextBlock(" commented on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetCommentDeleted)
				{
					eventBlock.Header.Add(new EventTextBlock(" deleted a comment on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetCommentUpdated)
				{
					eventBlock.Header.Add(new EventTextBlock(" updated a comment on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetLike)
				{
					eventBlock.Header.Add(new EventTextBlock(" approved commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetUnlike)
				{
					eventBlock.Header.Add(new EventTextBlock(" unapproved commit "));
				}

				eventBlock.Header.Add(nodeBlock);

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.PullRequestCreated || eventModel.Event == EventModel.Type.PullRequestRejected || eventModel.Event == EventModel.Type.PullRequestSuperseded ||
			         eventModel.Event == EventModel.Type.PullRequestUpdated || eventModel.Event == EventModel.Type.PullRequestFulfilled || eventModel.Event == EventModel.Type.PullRequestLike || eventModel.Event == EventModel.Type.PullRequestUnlike)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventModel.Type.PullRequestCreated)
					eventBlock.Header.Add(new EventTextBlock(" created pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestRejected)
					eventBlock.Header.Add(new EventTextBlock(" rejected pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestSuperseded)
					eventBlock.Header.Add(new EventTextBlock(" superseded pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestFulfilled)
					eventBlock.Header.Add(new EventTextBlock(" fulfilled pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestUpdated)
					eventBlock.Header.Add(new EventTextBlock(" updated pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestLike)
					eventBlock.Header.Add(new EventTextBlock(" approved pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestUnlike)
					eventBlock.Header.Add(new EventTextBlock(" unapproved pull request"));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.PullRequestCommentCreated || eventModel.Event == EventModel.Type.PullRequestCommentUpdated || eventModel.Event == EventModel.Type.PullRequestCommentDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventModel.Type.PullRequestCommentCreated)
				{
					eventBlock.Header.Add(new EventTextBlock(" commented on pull request"));
				}
				else if (eventModel.Event == EventModel.Type.PullRequestCommentUpdated)
				{
					eventBlock.Header.Add(new EventTextBlock(" updated comment in pull request"));
				}
				else if (eventModel.Event == EventModel.Type.PullRequestCommentDeleted)
				{
					eventBlock.Header.Add(new EventTextBlock(" deleted comment in pull request"));
				}

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.IssueComment)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" commented on issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.IssueUpdated)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" updated issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.IssueReported)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" reported issue"));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}

				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.StartFollowUser)
			{
				eventBlock.Header.Add(new EventTextBlock(" started following a user"));
			}
			else if (eventModel.Event == EventModel.Type.StopFollowUser)
			{
				eventBlock.Header.Add(new EventTextBlock(" stopped following a user"));
			}
			else if (eventModel.Event == EventModel.Type.StartFollowIssue)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" started following an issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.StopFollowIssue)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" stopped following an issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.StartFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" started following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.StopFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" stopped following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.CreateRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" created repository "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.DeleteRepo)
			{
				eventBlock.Header.Add(new EventTextBlock(" deleted a repository"));
			}
			else if (eventModel.Event == EventModel.Type.WikiUpdated)
			{
                if (eventModel.Repository == null)
                    return null;
                var description = eventModel.Description.ToObject<string>();
                eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, description);
				eventBlock.Header.Add(new EventTextBlock(" updated wiki page "));
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.WikiCreated)
			{
                if (eventModel.Repository == null)
                    return null;
                var description = eventModel.Description.ToObject<string>();
                eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, description);
				eventBlock.Header.Add(new EventTextBlock(" created wiki page "));
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.WikiDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" deleted wiki page "));
                var description = eventModel.Description.ToObject<string>();
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.ForkRepo)
			{
                if (eventModel.Repository == null)
                    return null;

				eventBlock.Header.Add(new EventTextBlock(" forked "));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);

				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				else
					eventBlock.Header.Add(new EventTextBlock("this repository"));
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("No such event handler for " + eventModel.Event);
				return null;
			}

			return eventBlock;
        }

		private EventTextBlock CreateRepositoryEventTextBlock(RepositoryDetailedModel repoModel)
        {
            //Most likely indicates a deleted repository
            if (repoModel == null)
                return new EventTextBlock("Unknown Repository");
            if (repoModel.Name == null)
                return new EventTextBlock("<Deleted Repository>");
			return new EventAnchorBlock(repoModel.Owner + "/" + repoModel.Name, () => GoToRepository(repoModel));
        }

		private EventTextBlock CommitBlock(EventModel e)
		{
			var node = e.Node;
			if (string.IsNullOrEmpty(node))
				return null;
			node = node.Substring(0, node.Length > 6 ? 6 : node.Length);
			return new EventAnchorBlock(node, () => GoToChangeset(e.Repository.Owner, e.Repository.Slug, e.Node));
		}
    }
}