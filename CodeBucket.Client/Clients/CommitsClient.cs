using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Client.V1;

namespace CodeBucket.Client
{
    public class CommitsClient
    {
        private readonly BitbucketClient _client;

        public CommitsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<Commit>> GetAll(string username, string repository, string branch = null)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commits/{Uri.EscapeDataString(branch)}";
            return _client.Get<Collection<Commit>>(uri);
        }

        public Task<Commit> Get(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}";
            return _client.Get<Commit>(uri);
        }

        public Task<Changeset> GetChangeset(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/changesets/{node}";
            return _client.Get<Changeset>(uri);
        }

        public Task<Collection<CommitComment>> GetComments(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/comments";
            return _client.Get<Collection<CommitComment>>(uri);
        }

        public Task<ChangesetComment> CreateComment(string username, string repository, string node, NewChangesetComment comment)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/changesets/{node}/comments";
            return _client.Post<ChangesetComment>(uri, comment);
        }

		public Task<string> GetPatch(string username, string repository, string node)
		{
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/approve";
			return _client.Get<string>(uri);
		}

        public Task Approve(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/approve";
            return _client.Post<CommitParticipant>(uri);
        }

        public Task Unapprove(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/approve";
            return _client.Delete(uri);
        }

        public Task<List<ChangesetDiff>> GetDiffStat(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/changesets/{node}/diffstat";
            return _client.Get<List<ChangesetDiff>>(uri);
        }
    }
}
