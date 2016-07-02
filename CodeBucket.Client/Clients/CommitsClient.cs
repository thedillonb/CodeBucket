using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Client.Models;
using CodeBucket.Client.Models.V2;

namespace CodeBucket.Client.Controllers
{
    public class CommitsClient
    {
        private readonly BitbucketClient _client;

        public CommitsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<CommitModel>> GetAll(string username, string repository, string branch = null)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commits/{Uri.EscapeDataString(branch)}";
            return _client.Get<Collection<CommitModel>>(uri);
        }

        public Task<CommitModel> Get(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commits/{node}";
            return _client.Get<CommitModel>(uri);
        }

        public Task<ChangesetModel> GetChangeset(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/changesets/{node}";
            return _client.Get<ChangesetModel>(uri);
        }

        public Task<Collection<CommitComment>> GetComments(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/comments";
            return _client.Get<Collection<CommitComment>>(uri);
        }

        public Task<ChangesetCommentModel> CreateComment(string username, string repository, string node, NewChangesetComment comment)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/comments";
            return _client.Post<ChangesetCommentModel>(uri, comment);
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
            return _client.Post<string>(uri);
        }

        public Task Unapprove(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/commit/{node}/approve";
            return _client.Delete(uri);
        }

        public Task<List<ChangesetDiffModel>> GetDiffStat(string username, string repository, string node)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/changesets/{node}/diffstat";
            return _client.Get<List<ChangesetDiffModel>>(uri);
        }
    }
}
