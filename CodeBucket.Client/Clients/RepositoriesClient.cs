using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Client.Clients
{
    public class RepositoriesClient
    {
        private readonly BitbucketClient _client;

        public RepositoriesClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<Repository>> GetRepositories(string username, string role = null)
        {
            var sb = new StringBuilder();
            sb.Append($"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}");
            if (role != null)
                sb.Append($"?role={role}");
            return _client.Get<Collection<Repository>>(sb.ToString());
        }

        public Task<Dictionary<string, BranchModel>> GetBranches(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<Dictionary<string, BranchModel>>($"{uri}/branches");
        }

        public Task<RepositorySearchModel> Search(string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories";
            return _client.Get<RepositorySearchModel>($"{uri}/?name={name}");
        }

        public Task<RepositoryDetailedModel> Get(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<RepositoryDetailedModel>(uri);
        }

        public Task<FollowersModel> GetFollowers(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<FollowersModel>($"{uri}/followers");
        }

        public Task<Dictionary<string, TagModel>> GetTags(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<Dictionary<string, TagModel>>($"{uri}/tags");
        }

        public Task<EventsModel> GetEvents(string username, string repository, int start = 0, int limit = 25)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<EventsModel>($"{uri}/events/?start={start}&limit={limit}");
        }

        public Task<RepositoryDetailedModel> Fork(string username, string repository, string name, string description = null, string language = null, bool? isPrivate = null)
        {
            var data = new Dictionary<string, string>();
            data.Add("name", name);
            if (description != null)
                data.Add("description", description);
            if (language != null)
                data.Add("language", language);
            if (isPrivate != null)
                data.Add("is_private", isPrivate.Value.ToString());

            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Post<RepositoryDetailedModel>($"{uri}/fork", data);
        }

        public Task<RepositoryFollowModel> ToggleFollow(string username, string repository)
        {
            var uri = $"https://bitbucket.org/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/follow";
            return _client.Post<RepositoryFollowModel>(uri, null);
        }

        public Task<PrimaryBranchModel> GetPrimaryBranch(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<PrimaryBranchModel>($"{uri}/main-branch");
        }

        public Task<FileModel> GetFile(string username, string repository, string branch, string file)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}"
                + $"/src/{Uri.EscapeUriString(branch)}/{file.TrimStart('/')}";
            return _client.Get<FileModel>(uri);
        }

        public Task<SourceModel> GetSourceInfo(string username, string repository, string branch, string path)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}"
                + $"/src/{Uri.EscapeUriString(branch)}/{path.TrimStart('/')}";
            return _client.Get<SourceModel>(uri);
        }

        public Task<WikiModel> GetWiki(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/wiki";
            return _client.Get<WikiModel>(uri);
        }
    }
}
