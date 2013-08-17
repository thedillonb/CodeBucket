using System;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class FollowingRepositoryController : RepositoryController
    {
        public FollowingRepositoryController()
            : base(string.Empty)
        {
            ShowOwner = true;
            Title = "Following".t();
        }

        protected override object OnUpdateModel(bool forced)
        {
            return Application.Client.Account.GetRepositories(forced);
        }
    }
}

