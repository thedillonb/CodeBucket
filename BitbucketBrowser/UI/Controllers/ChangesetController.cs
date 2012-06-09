using System;
using BitbucketSharp.Models;


namespace BitbucketBrowser.UI
{
    public class ChangesetController
    {
        public ChangesetController()
        {
        }
    }

    public class ChangesetInfoController : Controller<ChangesetModel>
    {
        public string Node { get; private set; }

        public string User { get; private set; }

        public string Slug { get; private set; }

        public ChangesetInfoController(string user, string slug, string node)
            : base(true, false)
        {
            Node = node;
            User = user;
            Slug = slug;
        }

        protected override void OnRefresh()
        {

        }

        protected override ChangesetModel OnUpdate()
        {
            var client = new BitbucketSharp.Client("thedillonb", "djames");
            return client.Users[User].Repositories[Slug].Changesets[Node].GetInfo();
        }
    }
}

