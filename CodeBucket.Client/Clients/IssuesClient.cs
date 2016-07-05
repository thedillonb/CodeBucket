using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Client
{
    public class IssuesClient
    {
        protected BitbucketClient Client { get; }

        public IssuesClient(BitbucketClient client)
        {
            Client = client;
        }

        public Task<V1.IssueCollection> Search(string username, string repository, string search)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<V1.IssueCollection>($"{uri}/?search={Uri.EscapeDataString(search)}");
        }

        public Task<V1.IssueCollection> GetAll(string username, string repository, int start = 0, int limit = 15, IEnumerable<Tuple<string, string>> search = null)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues" +
                $"?start={start}&limit={limit}";

            var searchStr =
                string.Join("&", (search ?? Enumerable.Empty<Tuple<string, string>>())
                            .Select(x => $"{x.Item1}={Uri.EscapeDataString(x.Item2)}"));

            return Client.Get<V1.IssueCollection>($"{uri}{searchStr}");
        }

        public Task<List<V1.IssueComponent>> GetComponents(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<V1.IssueComponent>>($"{uri}/components");
        }

        public Task<List<V1.IssueVersion>> GetVersions(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<V1.IssueVersion>>($"{uri}/versions");
        }

        public Task<List<V1.IssueMilestone>> GetMilestones(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<V1.IssueMilestone>>($"{uri}/milestones");
        }

        public Task<V1.Issue> Create(string username, string repository, V1.NewIssue issue)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Post<V1.Issue>(uri, issue);
        }

        public Task<V1.Issue> Get(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Get<V1.Issue>(uri);
        }

        public Task<V1.Followers> GetFollowers(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Get<V1.Followers>($"{uri}/followers");
        }

        public Task Delete(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Delete(uri);
        }

        public Task<V1.Issue> UpdateMilestone(string username, string repository, int id, string milestone)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<V1.Issue>(uri, new { milestone });
        }

        public Task<V1.Issue> UpdateVersion(string username, string repository, int id, string version)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<V1.Issue>(uri, new { version });
        }

        public Task<V1.Issue> UpdateComponent(string username, string repository, int id, string component)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<V1.Issue>(uri, new { component });
        }

        public Task<List<V1.IssueComment>> GetComments(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}/comments";
            return Client.Get<List<V1.IssueComment>>(uri);
        }

        public Task<V1.IssueComment> CreateComment(string username, string repository, int id, string content)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}/comments";
            return Client.Post<V1.IssueComment>(uri, new { content });
        }
    }
}
