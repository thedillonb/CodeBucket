using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Source;
using System.Linq;
using CodeBucket.Controllers;

namespace CodeBucket.Bitbucket.Controllers
{
    public class TagController : ListController<TagController.TagModel>
    {
        public string User { get; private set; }
        public string Repo { get; private set; }

        public TagController(string user, string repo)
            : base(true)
        {
            User = user;
            Repo = repo;
            Title = "Tags";
            SearchPlaceholder = "Search Tags";
        }

        protected override List<TagModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var d = Application.Client.Users[User].Repositories[Repo].GetTags(force);
            nextPage = -1;
            return d.Select(x => new TagModel { Name = x.Key, Node = x.Value.Node }).OrderBy(x => x.Name).ToList();
        }

        protected override Element CreateElement(TagModel obj)
        {
            var element = new StyledElement(obj.Name);
            element.Tapped += () => NavigationController.PushViewController(new SourceController(User, Repo, obj.Node), true);
            return element;
        }

        /// <summary>
        /// Bitbucket passes it's tag model around as a dictionary with the name as the key
        /// I don't care for that since I like my lists to inherit from ListController
        /// So I need to convert into an intermediate that has the two peices of information I need.
        /// </summary>
        public class TagModel
        {
            public string Name { get; set; }
            public string Node { get; set; }
        }
    }
}

