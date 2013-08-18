using System;
using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeBucket.Controllers;

namespace CodeBucket.ViewControllers
{
	public class FollowingRepositoriesViewController : RepositoriesViewController
    {
		public FollowingRepositoriesViewController()
            : base(string.Empty)
        {
            ShowOwner = true;
            Title = "Following".t();
			Controller = new FollowingRepositoriesController(this);
        }
    }
}

