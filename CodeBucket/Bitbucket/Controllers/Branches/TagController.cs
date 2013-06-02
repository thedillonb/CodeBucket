using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Source;
using System.Linq;
using CodeBucket.Controllers;

namespace CodeBucket.Bitbucket.Controllers
{
    public class TagController : Controller<Dictionary<string, TagModel>>
    {
        public string User { get; private set; }

        public string Repo { get; private set; }

        public TagController(string user, string repo)
            : base(true, true)
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Tags";
            User = user;
            Repo = repo;
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Tags";
        }

        protected override void OnRefresh()
        {
            var sec = new Section();
            
            if (Model.Keys.Count == 0)
            {
                sec.Add(new NoItemsElement("No Tags"));
            }
            else
            {
                foreach (var tagName in Model.Keys.OrderBy(x => x))
                {
					var element = new StyledElement(tagName);
					element.Tapped += () => NavigationController.PushViewController(new SourceController(User, Repo, Model[tagName].Node), true);
                    sec.Add(element);
                }
            }

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override Dictionary<string, TagModel> OnUpdate (bool forced)
        {
            return Application.Client.Users[User].Repositories[Repo].GetTags(forced);
        }

    }
}

