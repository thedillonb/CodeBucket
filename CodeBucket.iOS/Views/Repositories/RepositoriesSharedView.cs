using System;

namespace CodeBucket.Views.Repositories
{
    public class RepositoriesSharedView : BaseRepositoriesView
    {
        public override void ViewDidLoad()
        {
            Title = "Shared Repositories";
            base.ViewDidLoad();
        }
    }
}

