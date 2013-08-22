using System;
using BitbucketSharp.Models;
using CodeFramework.Controllers;
using System.Collections.Generic;

namespace CodeBucket.Controllers
{
    public class ExploreRepositoriesController : RepositoriesController
    {
        public bool Searched { get; private set; }

        public ExploreRepositoriesController(IListView<RepositoryDetailedModel> view)
            : base(view, string.Empty, "ExploreController")
        {
        }

        public override void Update(bool force)
        {
            //Don't do anything here...
            Model = new ListModel<RepositoryDetailedModel>() { Data = new List<RepositoryDetailedModel>() };
        }

        public void Search(string text)
        {
            Searched = true;
            Model = new ListModel<RepositoryDetailedModel> {
                Data = Application.Client.Repositories.Search(text).Repositories
            };
            Render();
        }

    }
}

