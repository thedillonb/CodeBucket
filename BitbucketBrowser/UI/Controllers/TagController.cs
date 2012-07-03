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
            Root.Add(new Section());
        }

        protected override void OnRefresh ()
        {
            List<Element> el = new List<Element>(Model.Count);
            foreach (var k in Model.Keys)
            {
                var element = new StyledElement(k)
                                                      { Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator };
                element.Tapped += () => NavigationController.PushViewController(new SourceController(User, Repo, Model[k].Node), true);
                el.Add(element);
            }

            InvokeOnMainThread(delegate {
                Root[0].Clear();
                Root[0].AddAll(el);
            });
        }

        protected override Dictionary<string, TagModel> OnUpdate ()
        {
            return Application.Client.Users[User].Repositories[Repo].GetTags();
        }

    }
}

