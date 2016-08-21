using System;
using System.Collections.Generic;

namespace CodeBucket.Core.Data
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Key => Username + Domain;

        public string Username { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; } = "https://bitbucket.org";

        public bool StashAccount { get; set; } = false;

        public string AvatarUrl { get; set; }

        public string DefaultStartupView { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public bool DontShowTeamEvents { get; set; }

        private List<PinnedRepository> _pinnedRepositories = new List<PinnedRepository>();
        public List<PinnedRepository> PinnedRepositories
        {
            get { return _pinnedRepositories ?? new List<PinnedRepository>(); }
            set { _pinnedRepositories = value ?? new List<PinnedRepository>(); }
        }

        public bool ShowTeamEvents { get; set; } = true;

        public bool ExpandTeamsAndGroups { get; set; } = true;

        public bool RepositoryDescriptionInList { get; set; } = true;
    }
}