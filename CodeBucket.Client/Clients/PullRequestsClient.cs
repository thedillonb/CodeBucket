using System;
using CodeBucket.Client.Models.V2;
using CodeBucket.Client.Models;
using System.Threading.Tasks;

namespace CodeBucket.Client.Controllers
{
    public class PullRequestsClient
    {
        private readonly BitbucketClient _client;

        public PullRequestsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<PullRequestModel>> Get(string username, string repository, string state = "OPEN")
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/pullrequests";
            return _client.Get<Collection<PullRequestModel>>($"{uri}?state={state}");
		}

        public Task<PullRequestModel> Get(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" + 
                $"/pullrequests/{id}";
            return _client.Get<PullRequestModel>(uri);
        }

        public Task<PullRequestModel> Merge(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/merge";
            return _client.Post<PullRequestModel>(uri);
        }

        public Task<PullRequestModel> Decline(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/decline";
            return _client.Post<PullRequestModel>(uri);
        }

        public Task<Collection<PullRequestCommentModel>> GetComments(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/comments";
            return _client.Get<Collection<PullRequestCommentModel>>(uri);
        }

        public Task<Collection<CommitModel>> GetCommits(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/commits";
            return _client.Get<Collection<CommitModel>>(uri);
        }

        public Task<OldPullRequestCommentModel> AddComment(string username, string repository, int id, string content)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/comments";
            return _client.Post<OldPullRequestCommentModel>(uri, new { content });
        }
    }
}

