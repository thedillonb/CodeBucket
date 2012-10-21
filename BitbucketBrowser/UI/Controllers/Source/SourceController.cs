using MonoTouch.Dialog;
using BitbucketSharp.Models;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;


namespace BitbucketBrowser.UI.Controllers.Source
{
    public class SourceController : Controller<SourceModel>
    {
        public string Username { get; private set; }

        public string Slug { get; private set; }

        public string Branch { get; private set; }

        public string Path { get; private set; }

        public SourceController(string username, string slug, string branch = "master", string path = "")
            : base(true, false)
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Username = username;
            Slug = slug;
            Branch = branch;
            Path = path;
            AutoHideSearch = true;
            EnableSearch = true;

            Title = string.IsNullOrEmpty(path) ? "Source" : path.Substring(path.LastIndexOf('/') + 1);
        }


        protected override void OnRefresh()
        {
            var sec = new Section();
            Model.Directories.ForEach(d => sec.Add(new StyledElement(d,
                                                                     () => NavigationController.PushViewController(new SourceController(Username, Slug, Branch, Path + "/" + d), true),
                                                                     Images.Folder)));

            Model.Files.ForEach(f =>
            {
                var i = f.Path.LastIndexOf('/') + 1;
                var p = f.Path.Substring(i);
                sec.Add(new StyledElement(p, () => NavigationController.PushViewController(
                                          new SourceInfoController(Username, Slug, Branch, f.Path) { Title = p }, true),
                                          Images.File));
            });

            if (sec.Count == 0)
            {
                sec.Add(new NoItemsElement());
            }

            InvokeOnMainThread(delegate
            {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override SourceModel OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].Repositories[Slug].Branches[Branch].Source[Path].GetInfo(forced);
        }

    }
}

