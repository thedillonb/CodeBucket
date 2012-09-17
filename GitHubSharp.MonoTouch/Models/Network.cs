using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GitHubSharp.Models
{
    public class NetworkMeta
    {
        public List<List<List<int>>> Spacemap { get; set; }
        public int Focus { get; set; }
        public string Nethash { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<NetworkUser> Users { get; set; }
    }

    public class NetworkUser
    {
        public string Name { get; set; }
        public string Repo { get; set; }
        public List<NetworkUserHeadInfo> Heads { get; set; }
    }

    public class NetworkUserHeadInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class NetworkBlock
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
    }

    public class NetworkChunk
    {
        public string Author { get; set; }
        public int Time { get; set; }
        public string Id { get; set; }
        public DateTime Date{ get; set; }
        public string Gravatar { get; set; }
        public int Space { get; set; }
        public string Message { get; set; }
        public string Login { get; set; }
        public List<List<string>> Parents { get; set; }
    }
}
