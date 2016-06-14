using System.Collections.Immutable;
using BitbucketSharp.Models;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.Stores
{
    public interface IMilestoneStore
    {
        LoadableResource<ImmutableList<IssueMilestone>> Milestones { get; }
    }

    public interface IVersionStore
    {
        LoadableResource<ImmutableList<IssueVersion>> Versions { get; }
    }

    public interface IComponentStore
    {
        LoadableResource<ImmutableList<IssueComponent>> Components { get; }
    }

    public class RepositoryStore : IMilestoneStore
    {
        public string Username { get; }

        public string RepositoryName { get; }

        public LoadableResource<Repository> Repository { get; }

        public LoadableResource<ImmutableList<IssueMilestone>> Milestones { get; }

        public LoadableResource<ImmutableList<IssueVersion>> Versions { get; }

        public LoadableResource<ImmutableList<IssueComponent>> Components { get; }

        public RepositoryStore(string username, string repositoryName, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Username = username;
            RepositoryName = repositoryName;

            Milestones = new LoadableResource<ImmutableList<IssueMilestone>>(async () =>
            {
                var items = await applicationService.Client.Repositories.Issues.GetMilestones(username, repositoryName);
                return items.ToImmutableList();
            }, ImmutableList<IssueMilestone>.Empty);

            Versions = new LoadableResource<ImmutableList<IssueVersion>>(async () =>
            {
                var items = await applicationService.Client.Repositories.Issues.GetVersions(username, repositoryName);
                return items.ToImmutableList();
            }, ImmutableList<IssueVersion>.Empty);

            //Components = new LoadableResource<ImmutableList<IssueComponent>>(async () =>
            //{
            //    var items = await applicationService.Client.Repositories.Issues.GetComponents(username, repositoryName);
            //    return items.ToImmutableList();
            //}, ImmutableList<IssueComponent>.Empty);
        }
    }
}

