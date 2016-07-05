using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodeBucket.Client
{
    public class UsersClient
    {
        private readonly BitbucketClient _client;

        public UsersClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<User> GetUser(string username)
        {
            return _client.Get<User>($"{BitbucketClient.ApiUrl2}/users/{Uri.EscapeDataString(username)}");
        }

        public Task<User> GetCurrent()
        {
            return _client.Get<User>($"{BitbucketClient.ApiUrl2}/user");
        }

        public Task<V1.EventCollection> GetEvents(string username, int start = 0, int limit = 25)
        {
            return _client.Get<V1.EventCollection>($"{BitbucketClient.ApiUrl}/users/{Uri.EscapeDataString(username)}/events/?start={start}&limit={limit}");
        }
		
        public Task<Collection<User>> GetFollowers(string username)
		{
            return _client.Get<Collection<User>>($"{BitbucketClient.ApiUrl2}/users/{Uri.EscapeDataString(username)}/followers");
		}

		public Task<Collection<User>> GetFollowing(string username, int limit = 100)
		{
            var uri = $"{BitbucketClient.ApiUrl2}/users/{Uri.EscapeDataString(username)}/following?pagelen={limit}";
			return _client.Get<Collection<User>>(uri);
		}

        public Task<List<Repository>> GetCurrentUserRepositoriesFollowing()
        {
            return _client.Get<List<Repository>>($"{BitbucketClient.ApiUrl}/user/follows");
        }

        public Task<V1.AccountPrivileges> GetCurrentUserPrivileges()
        {
            return _client.Get<V1.AccountPrivileges>($"{BitbucketClient.ApiUrl}/user/privileges");
        }
    }
}
