using BitbucketBrowser.Data;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers.Repositories
{
    public class RepositoryController : ListController<RepositoryModel>
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true)
            : base(push)
        {
            Title = "Repositories";
            Username = username;
            ShowOwner = true;
        }

        protected override List<RepositoryModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var a = Application.GitHubClient.API.ListRepositories(Username);
            nextPage = a.Next == null ? -1 : currentPage + 1;
            return a.Data.OrderBy(x => x.Name).ToList();
        }
        
        protected override Element CreateElement(RepositoryModel x)
        {
            var sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
            sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x.Owner.Login, x.Name), true);
            return sse;
        }
    }
}
