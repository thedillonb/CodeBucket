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

        public List<Models.Repository> ListRepositories()
        {
            return _client.Get<List<Models.Repository>>("/users/" + _client.Username + "/repos");
        }
    }
}

