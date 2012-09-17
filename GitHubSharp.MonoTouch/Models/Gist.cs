using System;
using System.Collections.Generic;

namespace GitHubSharp.Models
{
    public class Gist
    {
        public string url { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public bool @public { get; set; }
        public BasicUser user { get; set; }
        public Dictionary<string, GistFile> files { get; set; }
        public int comments { get; set; }
        public string html_url { get; set; }
        public string git_pull_url { get; set; }
        public string git_push_url { get; set; }
        public DateTime created_at { get; set; }
        public List<Fork> forks { get; set; }
        public List<History> history { get; set; }
    }

    public class GistFile
    {
        public int size { get; set; }
        public string filename { get; set; }
        public string raw_url { get; set; }
        public string content { get; set; }
    }

    public class GistToCreateOrEdit
    {
        public string description { get; set; }
        public bool @public { get; set; }
        public Dictionary<string, GistFileForCreation> files { get; set; }
    }

    public class GistFileForCreation
    {
        public string content { get; set; }
    }

}

