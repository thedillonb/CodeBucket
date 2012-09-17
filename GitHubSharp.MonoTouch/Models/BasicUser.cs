using System;

namespace GitHubSharp.Models
{
    public class BasicUser
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
    }
}

