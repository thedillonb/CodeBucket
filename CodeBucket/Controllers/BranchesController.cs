using System;
using CodeFramework.Controllers;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Controllers
{
    public class BranchesController : Controller<List<BranchModel>>
    {
        private readonly string _username;
        private readonly string _slug;

        public BranchesController(IView<List<BranchModel>> view, string username, string slug)
            : base(view)
        {
            _username = username;
            _slug = slug;
        }

        public override void Update(bool force)
        {
            Model = Application.Client.Users[_username].Repositories[_slug].Branches.GetBranches(force).Values.OrderBy(x => x.Branch).ToList();
        }
    }
}

