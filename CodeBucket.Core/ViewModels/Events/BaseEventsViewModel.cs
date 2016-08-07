using System;
using System.Linq;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.ViewModels.PullRequests;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Core.Utils;
using Humanizer;
using CodeBucket.Core.ViewModels.Wiki;
using System.Reactive.Linq;
using CodeBucket.Client.V1;
using Newtonsoft.Json;

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

        public string SearchText { get; set; }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        protected BaseEventsViewModel()
        {
            Title = "Events";

            var eventItems = new ReactiveList<EventItemViewModel>(resetChangeThreshold: 10);
            Items = eventItems.CreateDerivedCollection(x => x);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                HasMore = false;
                nextPage = 0;
                eventItems.Clear();
                var events = await GetEvents(nextPage, 40);
                nextPage += events.Events.Count;
                eventItems.AddRange(events.Events.Select(TryCatchEventBlockCreation).Where(x => x != null));
                HasMore = nextPage < events.Count;
            });

            var hasMoreObs = this.WhenAnyValue(x => x.HasMore);
            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(hasMoreObs, async _ =>
            {
                HasMore = false;
                var events = await GetEvents(nextPage, 40);
                nextPage += events.Events.Count;
                eventItems.AddRange(events.Events.Select(TryCatchEventBlockCreation).Where(x => x != null));
                HasMore = nextPage < events.Count;
            });

            LoadCommand.IsExecuting.CombineLatest(eventItems.IsEmptyChanged, (x, y) => !x && y)
                       .ToProperty(this, x => x.IsEmpty, out _isEmpty);
        }

        protected abstract Task<EventCollection> GetEvents(int start, int limit);

		private void GoToCommits(Repository repoModel, string branch)
        {
			if (branch != null)
                NavigateTo(new CommitsViewModel(repoModel.Owner, repoModel.Slug, branch));
			else
                NavigateTo(BranchesViewModel.ForCommits(repoModel.Owner, repoModel.Slug));
        }

		private void GoToRepository(Repository eventModel)
        {
			if (eventModel == null)
				return;
            NavigateTo(new RepositoryViewModel(eventModel.Owner, eventModel.Slug));
        }

        private void GoToRepositoryIssues(Repository eventModel)
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

		private void GoToRepositoryWiki(Repository repository, string page)
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
		private void GoToPullRequests(Repository repo)
        {
			if (repo == null) return;
            NavigateTo(new PullRequestsViewModel(repo.Owner, repo.Slug));
        }

		private void GoToChangeset(string owner, string name, string sha)
        {
            NavigateTo(new CommitViewModel(owner, name, sha));
        }

        private EventItemViewModel TryCatchEventBlockCreation(EventItem eventModel)
        {
            try
            {
                return CreateEventEventTextBlocks(eventModel);
            }
            catch
            {
                return null;
            }
        }

        private EventItemViewModel CreateEventEventTextBlocks(EventItem eventModel)
        {
            var avatar = new Avatar(eventModel.User?.Avatar);
            var createdOn = eventModel.UtcCreatedOn.Humanize();
            var eventType = eventModel.Event;
            var eventBlock = new EventItemViewModel(avatar, eventType, createdOn);
			var username = eventModel.User != null ? eventModel.User.Username : null;
            var description = eventModel.Description?.ToString() ?? string.Empty;


            // Insert the actor
            eventBlock.Header.Add(new EventAnchorBlock(username, () => GoToUser(username)));


			if (eventModel.Event == EventItem.Type.Pushed)
			{
                if (eventModel.Repository == null)
                    return null;

                var data = JsonConvert.DeserializeObject<PushedEventDescriptionModel>(description);
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

                        var repoName = eventModel.Repository.Name?.ToLower();
						eventBlock.Body.Add(new EventAnchorBlock(shortSha, () => GoToChangeset(eventModel.Repository.Owner, repoName, sha)));
						eventBlock.Body.Add(new EventTextBlock(" - " + desc + "\n"));
					}

                    eventBlock.Multilined = true;
				}
				return eventBlock;
			}

			if (eventModel.Event == EventItem.Type.Commit)
			{
                if (eventModel.Repository == null)
                    return null;

				var node = eventModel.Node.Substring(0, eventModel.Node.Length > 6 ? 6 : eventModel.Node.Length);
                var repoName = eventModel.Repository.Name?.ToLower();
                eventBlock.Tapped = () => GoToChangeset(eventModel.Repository.Owner, repoName, eventModel.Node);
				eventBlock.Header.Add(new EventTextBlock(" commited "));
				eventBlock.Header.Add(new EventAnchorBlock(node, eventBlock.Tapped));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
                var desc = string.IsNullOrEmpty(description) ? "" : description.Replace("\n", " ").Trim();
				eventBlock.Body.Add(new EventTextBlock(desc));
			}
			else if (eventModel.Event == EventItem.Type.ChangeSetCommentCreated || eventModel.Event == EventItem.Type.ChangeSetCommentDeleted ||
			         eventModel.Event == EventItem.Type.ChangeSetCommentUpdated || eventModel.Event == EventItem.Type.ChangeSetLike || eventModel.Event == EventItem.Type.ChangeSetUnlike)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Slug, eventModel.Node);
				var nodeBlock = CommitBlock(eventModel);

				if (eventModel.Event == EventItem.Type.ChangeSetCommentCreated)
				{
					eventBlock.Header.Add(new EventTextBlock(" commented on commit "));
				}
				else if (eventModel.Event == EventItem.Type.ChangeSetCommentDeleted)
				{
					eventBlock.Header.Add(new EventTextBlock(" deleted a comment on commit "));
				}
				else if (eventModel.Event == EventItem.Type.ChangeSetCommentUpdated)
				{
					eventBlock.Header.Add(new EventTextBlock(" updated a comment on commit "));
				}
				else if (eventModel.Event == EventItem.Type.ChangeSetLike)
				{
					eventBlock.Header.Add(new EventTextBlock(" approved commit "));
				}
				else if (eventModel.Event == EventItem.Type.ChangeSetUnlike)
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
			else if (eventModel.Event == EventItem.Type.PullRequestCreated || eventModel.Event == EventItem.Type.PullRequestRejected || eventModel.Event == EventItem.Type.PullRequestSuperseded ||
			         eventModel.Event == EventItem.Type.PullRequestUpdated || eventModel.Event == EventItem.Type.PullRequestFulfilled || eventModel.Event == EventItem.Type.PullRequestLike || eventModel.Event == EventItem.Type.PullRequestUnlike)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventItem.Type.PullRequestCreated)
					eventBlock.Header.Add(new EventTextBlock(" created pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestRejected)
					eventBlock.Header.Add(new EventTextBlock(" rejected pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestSuperseded)
					eventBlock.Header.Add(new EventTextBlock(" superseded pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestFulfilled)
					eventBlock.Header.Add(new EventTextBlock(" fulfilled pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestUpdated)
					eventBlock.Header.Add(new EventTextBlock(" updated pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestLike)
					eventBlock.Header.Add(new EventTextBlock(" approved pull request"));
				else if (eventModel.Event == EventItem.Type.PullRequestUnlike)
					eventBlock.Header.Add(new EventTextBlock(" unapproved pull request"));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventItem.Type.PullRequestCommentCreated || eventModel.Event == EventItem.Type.PullRequestCommentUpdated || eventModel.Event == EventItem.Type.PullRequestCommentDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventItem.Type.PullRequestCommentCreated)
				{
					eventBlock.Header.Add(new EventTextBlock(" commented on pull request"));
				}
				else if (eventModel.Event == EventItem.Type.PullRequestCommentUpdated)
				{
					eventBlock.Header.Add(new EventTextBlock(" updated comment in pull request"));
				}
				else if (eventModel.Event == EventItem.Type.PullRequestCommentDeleted)
				{
					eventBlock.Header.Add(new EventTextBlock(" deleted comment in pull request"));
				}

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventItem.Type.IssueComment)
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
			else if (eventModel.Event == EventItem.Type.IssueUpdated)
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
			else if (eventModel.Event == EventItem.Type.IssueReported)
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
			else if (eventModel.Event == EventItem.Type.StartFollowUser)
			{
				eventBlock.Header.Add(new EventTextBlock(" started following a user"));
			}
			else if (eventModel.Event == EventItem.Type.StopFollowUser)
			{
				eventBlock.Header.Add(new EventTextBlock(" stopped following a user"));
			}
			else if (eventModel.Event == EventItem.Type.StartFollowIssue)
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
			else if (eventModel.Event == EventItem.Type.StopFollowIssue)
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
			else if (eventModel.Event == EventItem.Type.StartFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" started following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventItem.Type.StopFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" stopped following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventItem.Type.CreateRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" created repository "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventItem.Type.DeleteRepo)
			{
				eventBlock.Header.Add(new EventTextBlock(" deleted a repository"));
			}
			else if (eventModel.Event == EventItem.Type.WikiUpdated)
			{
                if (eventModel.Repository == null)
                    return null;
                eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, description);
				eventBlock.Header.Add(new EventTextBlock(" updated wiki page "));
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventItem.Type.WikiCreated)
			{
                if (eventModel.Repository == null)
                    return null;
                eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, description);
				eventBlock.Header.Add(new EventTextBlock(" created wiki page "));
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventItem.Type.WikiDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new EventTextBlock(" deleted wiki page "));
                eventBlock.Header.Add(new EventAnchorBlock(description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new EventTextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryEventTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventItem.Type.ForkRepo)
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

		private EventTextBlock CreateRepositoryEventTextBlock(Repository repoModel)
        {
            //Most likely indicates a deleted repository
            if (repoModel == null)
                return new EventTextBlock("Unknown Repository");
            if (repoModel.Name == null)
                return new EventTextBlock("<Deleted Repository>");
			return new EventAnchorBlock(repoModel.Owner + "/" + repoModel.Name, () => GoToRepository(repoModel));
        }

		private EventTextBlock CommitBlock(EventItem e)
		{
			var node = e.Node;
			if (string.IsNullOrEmpty(node))
				return null;
			node = node.Substring(0, node.Length > 6 ? 6 : node.Length);
			return new EventAnchorBlock(node, () => GoToChangeset(e.Repository.Owner, e.Repository.Slug, e.Node));
		}
    }
}