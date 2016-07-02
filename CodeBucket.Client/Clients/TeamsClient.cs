using System;
using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Client.Clients
{
    public class TeamsClient
    {
        private readonly BitbucketClient _client;

        public TeamsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<Collection<User>> GetAll(string role = "member")
        {
            var uri = $"{BitbucketClient.ApiUrl2}/teams?role={role}";
            return _client.Get<Collection<User>>(uri);
        }

        public Task<Collection<User>> GetMembers(string teamName)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/teams/{Uri.EscapeDataString(teamName)}/members";
            return _client.Get<Collection<User>>(uri);
        }

        public Task<User> Get(string teamName)
        {
            var uri = $"{BitbucketClient.ApiUrl2}/teams/{Uri.EscapeDataString(teamName)}";
            return _client.Get<User>(uri);
        }
    }
}
