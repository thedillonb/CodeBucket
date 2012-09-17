using System;

namespace GitHubSharp.Models
{
    public class Repository
    {
        public string URL { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        public string Name { get; set; }
        public BasicUser Owner { get; set; }
        public bool Fork { get; set; }
        public bool Private { get; set; }
        public int OpenIssues { get; set; }
        public int Watchers { get; set; }
        public int Forks { get; set; }
        public string Source { get; set; }
        public string Parent { get; set; }

        public string WatchersURL { get { return string.Format("http://github.com/{0}/{1}/watchers", Owner, Name); } }
        public string DownloadURL { get { return string.Format("http://github.com/{0}/{1}/zipball/master", Owner, Name); } }
        public string ForksURL { get { return string.Format("http://github.com/{0}/{1}/network/members", Owner, Name); } }
        public string IssuesURL { get { return string.Format("http://github.com/{0}/{1}/issues", Owner, Name); } }
        public string WikiURL { get { return string.Format("http://wiki.github.com/{0}/{1}", Owner, Name); } }
        public string GraphsURL { get { return string.Format("http://github.com/{0}/{1}/graphs", Owner, Name); } }
        public string ForkQuoueURL { get { return string.Format("http://github.com/{0}/{1}/forkqueue", Owner, Name); } }
        public string GitCloneURL { get { return string.Format("git://github.com/{0}/{1}.git", Owner, Name); } }
        public string HttpCloneURL { get { return string.Format("http://github.com/{0}/{1}.git", Owner, Name); } }
        public string ForkURL { get { return string.Format("http://github.com/{0}/{1}/fork", Owner, Name); } }
        public string WatchURL { get { return string.Format("http://github.com/{0}/{1}/toggle_watch", Owner, Name); } }
    }

    public class RepositoryFromSearch
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Followers { get; set; }
        public string Username { get; set; }
        public string Language { get; set; }
        public bool Fork { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Pushed { get; set; }
        public int Forks { get; set; }
        public string Description { get; set; }
        public float Score { get; set; }
        public DateTime Created { get; set; }
    }

    public class Language
    {
        public string Name { get; set; }
        public int CalculatedBytes { get; set; }
    }

    public class TagOrBranch
    {
        public string Name { get; set; }
        public string Sha { get; set; }
    }
}
