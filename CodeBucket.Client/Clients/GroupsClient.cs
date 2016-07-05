using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBucket.Client
{
    public class GroupsClient
    {
        private readonly BitbucketClient _client;

        public GroupsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<List<V1.Group>> GetGroups(string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/groups/{Uri.EscapeDataString(name)}";
            return _client.Get<List<V1.Group>>(uri);
        }

        public Task<List<V1.User>> GetMembers(string owner, string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/groups/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(name)}/members";
            return _client.Get<List<V1.User>>(uri);
        }
    }
}
