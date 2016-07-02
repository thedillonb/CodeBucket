using System.Collections.Generic;
using CodeBucket.Client.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Client.Clients
{
    public class IssuesClient
    {
        protected BitbucketClient Client { get; }

        public IssuesClient(BitbucketClient client)
        {
            Client = client;
        }

        public Task<IssuesModel> Search(string username, string repository, string search)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<IssuesModel>($"{uri}/?search={Uri.EscapeDataString(search)}");
        }

        public Task<IssuesModel> GetAll(string username, string repository, int start = 0, int limit = 15, IEnumerable<Tuple<string, string>> search = null)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues" +
                $"?start={start}&limit={limit}";

            var searchStr =
                string.Join("&", (search ?? Enumerable.Empty<Tuple<string, string>>())
                            .Select(x => $"{x.Item1}={Uri.EscapeDataString(x.Item2)}"));

            return Client.Get<IssuesModel>($"{uri}{searchStr}");
        }

        public Task<List<ComponentModel>> GetComponents(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<ComponentModel>>($"{uri}/components");
        }

        public Task<List<VersionModel>> GetVersions(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<VersionModel>>($"{uri}/versions");
        }

        public Task<List<MilestoneModel>> GetMilestones(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Get<List<MilestoneModel>>($"{uri}/milestones");
        }

        public Task<IssueModel> Create(string username, string repository, CreateIssueModel issue)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues";
            return Client.Post<IssueModel>(uri, issue);
        }

        public Task<IssueModel> Get(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Get<IssueModel>(uri);
        }

        public Task<FollowersModel> GetFollowers(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Get<FollowersModel>($"{uri}/followers");
        }

        public Task Delete(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Delete(uri);
        }

        public Task<IssueModel> UpdateMilestone(string username, string repository, int id, string milestone)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<IssueModel>(uri, new { milestone });
        }

        public Task<IssueModel> UpdateVersion(string username, string repository, int id, string version)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<IssueModel>(uri, new { version });
        }

        public Task<IssueModel> UpdateComponent(string username, string repository, int id, string component)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}";
            return Client.Put<IssueModel>(uri, new { component });
        }

        public Task<List<CommentModel>> GetComments(string username, string repository, int id)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}/comments";
            return Client.Get<List<CommentModel>>(uri);
        }

        public Task<CommentModel> CreateComment(string username, string repository, int id, string content)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/issues/{id}/comments";
            return Client.Post<CommentModel>(uri, new { content });
        }
    }
}
