using GitHubSharp.Models;
using System.Collections.Generic;
using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers.Source;
using MonoTouch.Dialog;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers
{
    public class TagController : ListController<TagModel>
    {
        public string User { get; private set; }
        public string Repo { get; private set; }

        public TagController(string user, string repo)
            : base(true)
        {
            Title = "Tags";
            User = user;
            Repo = repo;
        }

        protected override List<TagModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var d = Application.GitHubClient.API.GetTags(User, Repo);
            nextPage = -1;
            return d.Data;
        }
        
        protected override Element CreateElement(TagModel obj)
        {
            return new StyledElement(obj.Name, () => NavigationController.PushViewController(new SourceController(User, Repo, obj.Commit.Sha), true));
        }

    }
}


