using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.Core.ViewModels.User;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.PullRequests;
using CodeBucket.Core.ViewModels.Commits;
using Newtonsoft.Json;

namespace CodeBucket.Core.ViewModels.Events
{
    public abstract class BaseEventsViewModel : LoadableViewModel
    {
        private readonly CollectionViewModel<Tuple<EventModel, EventBlock>> _events = new CollectionViewModel<Tuple<EventModel, EventBlock>>();

        public CollectionViewModel<Tuple<EventModel, EventBlock>> Events
        {
            get { return _events; }
        }

        public bool ReportRepository
        {
            get;
            private set;
        }

        protected BaseEventsViewModel()
        {
            ReportRepository = true;
        }

//		public void Update(bool force)
//		{
//			var sizeRequest = GetTotalItemCount();
//			var items = CreateRequest(0, 100);
//
//
//			Model = new ListModel<EventModel> { Data = GetData() };
//			if (Model.Data.Count < _dataLimit)
//				return;
//
//			if (Model.Data.Count < sizeRequest)
//			{
//				Model.More = () => {
//					var data = GetData(Model.Data.Count);
//					Model.Data.AddRange(data);
//					if (Model.Data.Count >= sizeRequest || data.Count < _dataLimit)
//						Model.More = null;
//					Render();
//				};
//			}
//		}

		protected abstract List<EventModel> CreateRequest(int start, int limit);

		protected abstract int GetTotalItemCount();

//
//		protected virtual List<EventModel> GetData(int start = 0, int limit = _dataLimit)
//		{
//			var events = Application.Client.Users[Username].GetEvents(start, limit);
//			return events.Events.OrderByDescending(x => x.UtcCreatedOn).ToList();
//		}
//
//		protected virtual int GetTotalItemCount()
//		{
//			return Application.Client.Users[Username].GetEvents(0, 0).Count;
//		}

		protected override Task Load(bool forceDataRefresh)
        {
			return Task.Run(() => this.RequestModel(() => CreateRequest(0, 50), response => {
				//this.CreateMore(response, m => Events.MoreItems = m, d => Events.Items.AddRange(CreateDataFromLoad(d)));
                Events.Items.Reset(CreateDataFromLoad(response));
            }));
        }

        private IEnumerable<Tuple<EventModel, EventBlock>> CreateDataFromLoad(IEnumerable<EventModel> events)
        {
			return events.Select(x => new Tuple<EventModel, EventBlock>(x, CreateEventTextBlocks(x))).Where(x => x.Item2 != null);
        }


		private void GoToCommits(RepositoryDetailedModel repoModel, string branch)
        {
			if (branch != null)
			{
				ShowViewModel<CommitsViewModel>(new CommitsViewModel.NavObject
				{
					Username = repoModel.Owner,
					Repository = repoModel.Name,
					Branch = branch
				});
			}
			else
			{
				ShowViewModel<ChangesetBranchesViewModel>(new ChangesetBranchesViewModel.NavObject
				{
					Username = repoModel.Owner,
					Repository = repoModel.Name
				});
			}
        }

		private void GoToRepository(RepositoryDetailedModel eventModel)
        {
			if (eventModel == null)
				return;

            ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject
            {
				Username = eventModel.Owner,
				RepositorySlug = eventModel.Slug
            });
        }

		private void GoToRepositoryIssues(RepositoryDetailedModel eventModel)
		{
			if (eventModel == null)
				return;

			ShowViewModel<IssuesViewModel>(new IssuesViewModel.NavObject
				{
					Username = eventModel.Owner,
					Repository = eventModel.Slug
				});
		}

        private void GoToUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return;
            ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject {Username = username});
        }

		private void GoToRepositoryWiki(RepositoryDetailedModel repository, string page)
		{
			if (repository == null)
				return;

			ShowViewModel<Wiki.WikiViewModel>(new Wiki.WikiViewModel.NavObject
			{
				Username = repository.Owner,
				Repository = repository.Slug,
				Page = page
			});
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
		private void GoToPullRequest(RepositoryDetailedModel repo, ulong id)
        {
			if (repo == null)
				return;
            ShowViewModel<PullRequestViewModel>(new PullRequestViewModel.NavObject
            {
                Username = repo.Owner,
                Repository = repo.Name,
                Id = id
            });
        }

		private void GoToPullRequests(RepositoryDetailedModel repo)
        {
			if (repo == null)
				return;
            ShowViewModel<PullRequestsViewModel>(new PullRequestsViewModel.NavObject
            {
                Username = repo.Owner,
                Repository = repo.Name
            });
        }

		private void GoToChangeset(string owner, string name, string sha)
        {
			ShowViewModel<CommitViewModel>(new CommitViewModel.NavObject
            {
				Username = owner,
				Repository = name,
				Node = sha
            });
        }

        private EventBlock CreateEventTextBlocks(EventModel eventModel)
        {
            var eventBlock = new EventBlock();
			var username = eventModel.User != null ? eventModel.User.Username : null;

            // Insert the actor
			eventBlock.Header.Add(new AnchorBlock(username, () => GoToUser(username)));


			if (eventModel.Event == EventModel.Type.Pushed)
			{
                if (eventModel.Repository == null)
                    return null;

                var data = JsonConvert.DeserializeObject<PushedEventDescriptionModel>(eventModel.Description);

				if (eventModel.Repository != null)
					eventBlock.Tapped = () => GoToCommits(eventModel.Repository, null);

				eventBlock.Header.Add(new TextBlock(" pushed " + data.TotalCommits + " commit" + (data.TotalCommits > 1 ? "s" : string.Empty)));

                if (ReportRepository)
                {
					eventBlock.Header.Add(new TextBlock(" to "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
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

						eventBlock.Body.Add(new AnchorBlock(shortSha, () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Name, sha)));
						eventBlock.Body.Add(new TextBlock(" - " + desc + "\n"));
					}
				}
				return eventBlock;
			}

			if (eventModel.Event == EventModel.Type.Commit)
			{
                if (eventModel.Repository == null)
                    return null;

				var node = eventModel.Node.Substring(0, eventModel.Node.Length > 6 ? 6 : eventModel.Node.Length);
				eventBlock.Tapped = () => GoToChangeset(eventModel.Repository.Owner, eventModel.Repository.Name, eventModel.Node);
				eventBlock.Header.Add(new TextBlock(" commited "));
				eventBlock.Header.Add(new AnchorBlock(node, eventBlock.Tapped));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
				var desc = string.IsNullOrEmpty(eventModel.Description) ? "" : eventModel.Description.Replace("\n", " ").Trim();
				eventBlock.Body.Add(new TextBlock(desc));
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
					eventBlock.Header.Add(new TextBlock(" commented on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetCommentDeleted)
				{
					eventBlock.Header.Add(new TextBlock(" deleted a comment on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetCommentUpdated)
				{
					eventBlock.Header.Add(new TextBlock(" updated a comment on commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetLike)
				{
					eventBlock.Header.Add(new TextBlock(" approved commit "));
				}
				else if (eventModel.Event == EventModel.Type.ChangeSetUnlike)
				{
					eventBlock.Header.Add(new TextBlock(" unapproved commit "));
				}

				eventBlock.Header.Add(nodeBlock);

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.PullRequestCreated || eventModel.Event == EventModel.Type.PullRequestRejected || eventModel.Event == EventModel.Type.PullRequestSuperseded ||
			         eventModel.Event == EventModel.Type.PullRequestUpdated || eventModel.Event == EventModel.Type.PullRequestFulfilled || eventModel.Event == EventModel.Type.PullRequestLike || eventModel.Event == EventModel.Type.PullRequestUnlike)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventModel.Type.PullRequestCreated)
					eventBlock.Header.Add(new TextBlock(" created pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestRejected)
					eventBlock.Header.Add(new TextBlock(" rejected pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestSuperseded)
					eventBlock.Header.Add(new TextBlock(" superseded pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestFulfilled)
					eventBlock.Header.Add(new TextBlock(" fulfilled pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestUpdated)
					eventBlock.Header.Add(new TextBlock(" updated pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestLike)
					eventBlock.Header.Add(new TextBlock(" liked pull request"));
				else if (eventModel.Event == EventModel.Type.PullRequestUnlike)
					eventBlock.Header.Add(new TextBlock(" unliked pull request"));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.PullRequestCommentCreated || eventModel.Event == EventModel.Type.PullRequestCommentUpdated || eventModel.Event == EventModel.Type.PullRequestCommentDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToPullRequests(eventModel.Repository);

				if (eventModel.Event == EventModel.Type.PullRequestCommentCreated)
				{
					eventBlock.Header.Add(new TextBlock(" commented on pull request"));
				}
				else if (eventModel.Event == EventModel.Type.PullRequestCommentUpdated)
				{
					eventBlock.Header.Add(new TextBlock(" updated comment in pull request"));
				}
				else if (eventModel.Event == EventModel.Type.PullRequestCommentDeleted)
				{
					eventBlock.Header.Add(new TextBlock(" deleted comment in pull request"));
				}

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.IssueComment)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" commented on issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.IssueUpdated)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" updated issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.IssueReported)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" reported issue"));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}

				eventBlock.Tapped = () => GoToRepositoryIssues(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.StartFollowUser)
			{
				eventBlock.Header.Add(new TextBlock(" started following a user"));
			}
			else if (eventModel.Event == EventModel.Type.StopFollowUser)
			{
				eventBlock.Header.Add(new TextBlock(" stopped following a user"));
			}
			else if (eventModel.Event == EventModel.Type.StartFollowIssue)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" started following an issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.StopFollowIssue)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" stopped following an issue"));
				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.StartFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" started following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.StopFollowRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" stopped following "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.CreateRepo)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" created repository "));
				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);
			}
			else if (eventModel.Event == EventModel.Type.DeleteRepo)
			{
				eventBlock.Header.Add(new TextBlock(" deleted a repository"));
			}
			else if (eventModel.Event == EventModel.Type.WikiUpdated)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, eventModel.Description);
				eventBlock.Header.Add(new TextBlock(" updated wiki page "));
				eventBlock.Header.Add(new AnchorBlock(eventModel.Description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, eventModel.Description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.WikiCreated)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Tapped = () => GoToRepositoryWiki(eventModel.Repository, eventModel.Description);
				eventBlock.Header.Add(new TextBlock(" created wiki page "));
				eventBlock.Header.Add(new AnchorBlock(eventModel.Description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, eventModel.Description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.WikiDeleted)
			{
                if (eventModel.Repository == null)
                    return null;
				eventBlock.Header.Add(new TextBlock(" deleted wiki page "));
				eventBlock.Header.Add(new AnchorBlock(eventModel.Description.TrimStart('/'), () => GoToRepositoryWiki(eventModel.Repository, eventModel.Description)));

				if (ReportRepository)
				{
					eventBlock.Header.Add(new TextBlock(" in "));
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				}
			}
			else if (eventModel.Event == EventModel.Type.ForkRepo)
			{
                if (eventModel.Repository == null)
                    return null;

				eventBlock.Header.Add(new TextBlock(" forked "));
				eventBlock.Tapped = () => GoToRepository(eventModel.Repository);

				if (ReportRepository)
					eventBlock.Header.Add(CreateRepositoryTextBlock(eventModel.Repository));
				else
					eventBlock.Header.Add(new TextBlock("this repository"));
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("No such event handler for " + eventModel.Event);
				return null;
			}

			return eventBlock;
        }

		private TextBlock CreateRepositoryTextBlock(RepositoryDetailedModel repoModel)
        {
            //Most likely indicates a deleted repository
            if (repoModel == null)
                return new TextBlock("Unknown Repository");
            if (repoModel.Name == null)
                return new TextBlock("<Deleted Repository>");
			return new AnchorBlock(repoModel.Owner + "/" + repoModel.Name, () => GoToRepository(repoModel));
        }

		private TextBlock CommitBlock(EventModel e)
		{
			var node = e.Node;
			if (string.IsNullOrEmpty(node))
				return null;
			node = node.Substring(0, node.Length > 6 ? 6 : node.Length);
			return new AnchorBlock(node, () => GoToChangeset(e.Repository.Owner, e.Repository.Slug, e.Node));
		}


        public class EventBlock
        {
            public IList<TextBlock> Header { get; private set; }
            public IList<TextBlock> Body { get; private set; } 
            public Action Tapped { get; set; }

            public EventBlock()
            {
                Header = new List<TextBlock>(6);
                Body = new List<TextBlock>();
            }
        }

        public class TextBlock
        {
            public string Text { get; set; }

            public TextBlock()
            {
            }

            public TextBlock(string text)
            {
                Text = text;
            }
        }

        public class AnchorBlock : TextBlock
        {
            public AnchorBlock(string text, Action tapped) : base(text)
            {
                Tapped = tapped;
            }

            public Action Tapped { get; set; }

            public AnchorBlock(Action tapped)
            {
                Tapped = tapped;
            }
        }
    }
}