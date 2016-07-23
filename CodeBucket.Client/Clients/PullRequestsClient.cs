using System;
using System.Threading.Tasks;

namespace CodeBucket.Client
{
    public class PullRequestsClient
    {
        private readonly BitbucketClient _client;

        public PullRequestsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<PullRequest>> GetAll(string username, string repository, PullRequestState state, int pagelen = 50)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/pullrequests";
            return _client.Get<Collection<PullRequest>>($"{uri}?state={state.ToString().ToUpper()}&pagelen={pagelen}");
		}

        public Task<PullRequest> Get(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" + 
                $"/pullrequests/{id}";
            return _client.Get<PullRequest>(uri);
        }

        public Task<PullRequest> Merge(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/merge";
            return _client.PostForm<PullRequest>(uri);
        }

        public Task<PullRequest> Decline(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/decline";
            return _client.PostForm<PullRequest>(uri);
        }

        public Task<PullRequestParticipant> Approve(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/approve";
            return _client.Post<PullRequestParticipant>(uri);
        }

        public Task Unapprove(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/approve";
            return _client.Delete(uri);
        }

        public Task<Collection<PullRequestComment>> GetComments(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/comments";
            return _client.Get<Collection<PullRequestComment>>(uri);
        }

        public Task<PullRequestComment> GetComment(string username, string repository, int pullRequestId, int commentId)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{pullRequestId}/comments/{commentId}";
            return _client.Get<PullRequestComment>(uri);
        }

        public Task<Collection<Commit>> GetCommits(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/commits";
            return _client.Get<Collection<Commit>>(uri);
        }

        public Task<V1.PullRequestComment> AddComment(string username, string repository, int id, string content)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}" +
                $"/pullrequests/{id}/comments";
            return _client.Post<V1.PullRequestComment>(uri, new { content });
        }
    }
}

