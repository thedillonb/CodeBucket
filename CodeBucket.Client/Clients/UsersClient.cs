using CodeBucket.Client.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CodeBucket.Client;

namespace CodeBucket.Clients
{
    public class UsersClient
    {
        private readonly BitbucketClient _client;

        public UsersClient(BitbucketClient client)
        {
            _client = client;
        }

        public Task<UsersModel> GetUser(string username)
        {
            return _client.Get<UsersModel>($"{BitbucketClient.ApiUrl}/users/{Uri.EscapeDataString(username)}");
        }

        public Task<UsersModel> GetCurrent()
        {
            return _client.Get<UsersModel>($"{BitbucketClient.ApiUrl}/user");
        }

        public Task<EventsModel> GetEvents(string username, int start = 0, int limit = 25)
        {
            return _client.Get<EventsModel>($"{BitbucketClient.ApiUrl}/users/{Uri.EscapeDataString(username)}/events/?start={start}&limit={limit}");
        }
		
        public Task<FollowersModel> GetFollowers(string username)
		{
            return _client.Get<FollowersModel>($"{BitbucketClient.ApiUrl}/users/{Uri.EscapeDataString(username)}/followers");
		}

		public Task<Collection<User>> GetFollowing(string username, int limit = 100)
		{
            var uri = $"{BitbucketClient.ApiUrl2}/users/{Uri.EscapeDataString(username)}/following?pagelen={limit}";
			return _client.Get<Collection<User>>(uri);
		}

        public Task<List<RepositoryDetailedModel>> GetCurrentUserRepositoriesFollowing()
        {
            return _client.Get<List<RepositoryDetailedModel>>($"{BitbucketClient.ApiUrl}/user/follows");
        }

        public Task<AccountPrivileges> GetCurrentUserPrivileges()
        {
            return _client.Get<AccountPrivileges>($"{BitbucketClient.ApiUrl}/user/privileges");
        }
    }
}
