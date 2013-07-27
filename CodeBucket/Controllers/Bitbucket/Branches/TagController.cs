using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Source;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers
{
    public class TagController : BaseController
    {
        public List<TagModel> Model { get; set; }

        public string Username { get; private set; }

        public string Repo { get; private set; }

        public TagController(string user, string repo)
            : base(true)
        {
            Username = user;
            Repo = repo;
            Title = "Tags";
            SearchPlaceholder = "Search Tags";
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
        }

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { 
                    var tags = Application.Client.Users[Username].Repositories[Repo].GetTags(force);
                    Model = tags.Select(x => new TagModel { Name = x.Key, Node = x.Value.Node }).OrderBy(x => x.Name).ToList();
                });
            AddItems<TagModel>(Model, (o) => new StyledElement(o.Name, () => NavigationController.PushViewController(new SourceController(Username, Repo, o.Node), true)));
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

