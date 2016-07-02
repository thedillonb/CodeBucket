using System.Collections.Generic;
using System;

namespace CodeBucket.Client.Models
{
    public class UsersModel
    {
        public UserModel User { get; set; }
        public List<RepositoryDetailedModel> Repositories { get; set; }
    }

    public class UserModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsTeam { get; set; }
        public string Avatar { get; set; }
        public string ResourceUrl { get; set; }

        public override bool Equals(object obj)
        {
			var userModel = obj as UserModel;
			return userModel != null && string.Equals(Username, userModel.Username);
        }

        public override int GetHashCode()
        {
            return this.Username.GetHashCode();
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Kind { get; set; }
        public string Website { get; set; }
        public string DisplayName { get; set; }
        public string Location { get; set; }
        public DateTime CreatedOn { get; set; }
        public LinksModel Links { get; set; }

        public class LinksModel
        {
            public Link Avatar { get; set; }
        }
    }

    public class Collection<T>
    {
        public ulong Size { get; set; }
        public ulong Page { get; set; }
        public uint Pagelen { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<T> Values { get; set; }
    }
}
