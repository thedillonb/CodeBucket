using System;
using BitbucketSharp.Models;
using CodeFramework.Controllers;

namespace CodeBucket.Controllers
{
    public class FollowingRepositoriesController : RepositoriesController
    {
        public FollowingRepositoriesController(IListView<RepositoryDetailedModel> view)
            : base(view, string.Empty)
        {
        }

        public override void Update(bool force)
        {
            Model = new ListModel<RepositoryDetailedModel> {
                Data = Application.Client.Account.GetRepositories(force)
            };
        }
    }
}

