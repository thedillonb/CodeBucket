using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Client.Clients
{
    public class PrivilegesClient
    {
        protected BitbucketClient Client { get; }

        public PrivilegesClient(BitbucketClient client)
        {
            Client = client;
        }

        public Task<AccountPrivileges> GetUserPrivileges(string username)
        {
            var uri = $"{BitbucketClient.ApiUrl}/privileges/{Uri.EscapeDataString(username)}";
            return Client.Get<AccountPrivileges>(uri);
        }

        public Task<List<PrivilegeModel>> GetRepositoryPrivileges(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/privileges/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return Client.Get<List<PrivilegeModel>>(uri);
        }

        public Task<List<GroupPrivilegeModel>> GetRepositoryGroupPrivileges(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/group-privileges/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return Client.Get<List<GroupPrivilegeModel>>(uri);
        }
    }
}