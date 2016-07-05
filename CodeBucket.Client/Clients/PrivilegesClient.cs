using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBucket.Client
{
    public class PrivilegesClient
    {
        protected BitbucketClient Client { get; }

        public PrivilegesClient(BitbucketClient client)
        {
            Client = client;
        }

        public Task<V1.AccountPrivileges> GetUserPrivileges(string username)
        {
            var uri = $"{BitbucketClient.ApiUrl}/privileges/{Uri.EscapeDataString(username)}";
            return Client.Get<V1.AccountPrivileges>(uri);
        }

        public Task<List<V1.PrivilegeModel>> GetRepositoryPrivileges(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/privileges/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return Client.Get<List<V1.PrivilegeModel>>(uri);
        }

        public Task<List<V1.GroupPrivilege>> GetRepositoryGroupPrivileges(string username, string repository)
        {
            var uri = $"{BitbucketClient.ApiUrl}/group-privileges/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(repository)}";
            return Client.Get<List<V1.GroupPrivilege>>(uri);
        }
    }
}