using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeBucket.Client
{
    public class RepositoriesClient
    {
        private readonly BitbucketClient _client;

        public RepositoriesClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<Repository>> GetAll(string username, string role = null)
        {
            var sb = new StringBuilder();
            sb.Append($"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}");
            if (role != null)
                sb.Append($"?role={role}");
            return _client.Get<Collection<Repository>>(sb.ToString());
        }

        public async Task<IEnumerable<V1.GitReference>> GetBranches(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            var ret = await _client.Get<Dictionary<string, V1.GitReference>>($"{uri}/branches");
            foreach (var r in ret)
                r.Value.Name = r.Key;
            return ret.Values;
        }

        public Task<RepositorySearch> Search(string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories";
            return _client.Get<RepositorySearch>($"{uri}/?name={name}");
        }

        public Task<Repository> Get(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<Repository>(uri);
        }

        public Task<V1.Repository> Fork(string username, string repository, string name = null)
        {
            name = name ?? repository;
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/fork";
            return _client.PostForm<V1.Repository>(uri, new Dictionary<string, string> { { "name", name } });
        }

        public Task<V1.Followers> GetFollowers(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<V1.Followers>($"{uri}/followers");
        }

        public async Task<IEnumerable<V1.GitReference>> GetTags(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            var ret = await _client.Get<Dictionary<string, V1.GitReference>>($"{uri}/tags");
            foreach (var r in ret)
                r.Value.Name = r.Key;
            return ret.Values;
        }

        public Task<V1.EventCollection> GetEvents(string username, string repository, int start = 0, int limit = 25)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return _client.Get<V1.EventCollection>($"{uri}/events/?start={start}&limit={limit}");
        }

        public Task<Repository> Fork(string username, string repository, string name, string description = null, string language = null, bool? isPrivate = null)
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
            return _client.Post<Repository>($"{uri}/fork", data);
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

        public Task<V1.FileModel> GetFile(string username, string repository, string branch, string file)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}"
                + $"/src/{Uri.EscapeUriString(branch)}/{file.TrimStart('/')}";
            return _client.Get<V1.FileModel>(uri);
        }

        public Task<V1.SourceDirectory> GetSourceDirectory(string username, string repository, string branch, string path = null)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}"
                + $"/src/{Uri.EscapeUriString(branch)}/{path?.TrimStart('/')}";
            return _client.Get<V1.SourceDirectory>(uri);
        }

        public Task<V1.Wiki> GetWiki(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/wiki";
            return _client.Get<V1.Wiki>(uri);
        }

        public Task<Collection<User>> GetWatchers(string username, string repository)
        {
            return _client.Get<Collection<User>>($"{BitbucketClient.ApiUrl2}/repositories/" +
                                                 $"{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/watchers");
        }

        public Task<Collection<Repository>> GetForks(string username, string repository)
        {
            return _client.Get<Collection<Repository>>($"{BitbucketClient.ApiUrl2}/repositories/" +
                                                       $"{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}/forks");
        }

        public async Task GetRawFile(string username, string repository, string branch, string path, Stream output)
        {
            var uri = $"{BitbucketClient.ApiUrl}/repositories/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}"
                + $"/raw/{Uri.EscapeUriString(branch)}/{path?.TrimStart('/')}";
            var response = await _client.GetRaw(uri);
            await response.Content.CopyToAsync(output);
        }
    }
}
