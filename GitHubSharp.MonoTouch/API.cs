using System;
using System.Collections.Generic;

namespace GitHubSharp
{
    public class API
    {
        private Client _client;

        public API(Client client)
        {
            _client = client;
        }

        public List<Models.Repository> ListRepositories(string username = null)
        {
            if (username == null)
                return _client.Get<List<Models.Repository>>("/user/repos");
            else
                return _client.Get<List<Models.Repository>>("/users/" + username + "/repos");
        }
    }
}

