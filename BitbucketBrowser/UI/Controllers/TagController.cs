using System;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;


namespace BitbucketBrowser.UI
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
        }

        protected override void OnRefresh ()
        {
            var sec = new Section();
            foreach (var k in Model.Keys)
            {
                var element = new StyledElement(k)
                                                      { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
                element.Tapped += () => NavigationController.PushViewController(new SourceController(User, Repo, Model[k].Node), true);
                sec.Add(element);
            }

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override Dictionary<string, TagModel> OnUpdate ()
        {
            return Application.Client.Users[User].Repositories[Repo].GetTags();
        }

    }
}

