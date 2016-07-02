using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Client.Clients
{
    public class GroupsClient
    {
        private readonly BitbucketClient _client;

        public GroupsClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<List<GroupModel>> GetGroups(string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/groups/{Uri.EscapeDataString(name)}";
            return _client.Get<List<GroupModel>>(uri);
        }

        public Task<List<UserModel>> GetMembers(string name)
        {
            var uri = $"{BitbucketClient.ApiUrl}/groups/{Uri.EscapeDataString(name)}/members";
            return _client.Get<List<UserModel>>(uri);
        }
    }
}
