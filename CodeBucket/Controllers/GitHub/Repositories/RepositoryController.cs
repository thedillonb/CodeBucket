using CodeBucket.Controllers;
using CodeBucket.Elements;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.GitHub.Controllers.Repositories
{
    public class RepositoryController : ListController<RepositoryModel>
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true, bool refresh = true)
            : base(push, refresh)
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
