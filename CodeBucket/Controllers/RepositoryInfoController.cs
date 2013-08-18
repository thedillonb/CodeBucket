using System;
using CodeFramework.Controllers;
using BitbucketSharp.Models;

namespace CodeBucket.Controllers
{
    public class RepositoryInfoController : Controller<RepositoryDetailedModel>
    {
        public string User { get; private set; }

        public string Repo { get; private set; }

        public RepositoryInfoController(IView<RepositoryDetailedModel> view, string user, string repo)
            : base(view)
        {
            User = user;
            Repo = repo;
        }

        public override void Update(bool force)
        {
            Model = Application.Client.Users[User].Repositories[Repo].GetInfo(force);
        }
    }
}

