using BitbucketBrowser.Data;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers.Groups
{
	public class OrganizationsController : ListController<BasicUserModel>
	{
        public string Username { get; private set; }

        public OrganizationsController(string username, bool push = true) 
            : base(push)
		{
            Username = username;
            Title = "Organizations";
		}

        protected override Element CreateElement(BasicUserModel obj)
        {
            return new StyledElement(obj.Login, () => NavigationController.PushViewController(new GroupInfoController(obj.Login), true));
        }

        protected override List<BasicUserModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var f = Application.GitHubClient.API.GetOrganizations(Username);
            nextPage = -1;
            return f.Data.OrderBy(x => x.Login).ToList();
        }
	}
}

