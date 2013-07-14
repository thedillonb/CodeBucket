using System;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class FollowingRepositoryController : RepositoryController
    {
        public FollowingRepositoryController(bool push = true)
            : base(string.Empty, push, true)
        {
            Title = "Following";
            ShowOwner = true;
        }

        protected override List<RepositoryDetailedModel> GetData(bool force, int currentPage, out int nextPage)
        {
            nextPage = -1;
            return Application.Client.Account.GetRepositories(force);
        }
    }
}

