using System;

namespace GitHubSharp.Models
{
    public class UserInCollection
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Followers { get; set; }
        public string Username { get; set; }
        public string Language { get; set; }
        public string Fullname { get; set; }
        public int Repos { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Pushed { get; set; }
        public float Score { get; set; }
        public DateTime Created { get; set; }
    }

    public class User
    {
        public string GravatarId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PublicGistCount { get; set; }
        public int PublicRepoCount { get; set; }
        public string Blog { get; set; }
        public int Following { get; set; }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }

    public class UserAuthenticated : User
    {
        public int TotalPrivateRepoCount { get; set; }
        public int Collaborators { get; set; }
        public int DiskUsage { get; set; }
        public int OwnedPrivateRepoCount { get; set; }
        public int PrivateGistCount { get; set; }
        public UserAuthenticatedPlan Plan { get; set; }
    }

    public class UserAuthenticatedPlan
    {
        public string Name { get; set; }
        public int Collaborators { get; set; }
        public int Space { get; set; }
        public int PrivateRepos { get; set; }
    }

    public class PublicKey
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
    }

   
}
