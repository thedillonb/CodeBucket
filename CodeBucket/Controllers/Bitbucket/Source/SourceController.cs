using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;


namespace CodeBucket.Bitbucket.Controllers.Source
{
    public class SourceController : BaseController
    {
        public SourceModel Model { get; set; }

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
            SearchPlaceholder = "Search Files & Folders";

            Title = string.IsNullOrEmpty(path) ? "Source" : path.Substring(path.LastIndexOf('/') + 1);
        }

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[Username].Repositories[Slug].Branches[Branch].Source[Path].GetInfo(force); });

            var sec = new Section();
            Model.Directories.ForEach(d => sec.Add(new StyledElement(d, () => NavigationController.PushViewController(
                new SourceController(Username, Slug, Branch, Path + "/" + d), true), Images.Folder)));

            Model.Files.ForEach(f => {
                var i = f.Path.LastIndexOf('/') + 1;
                var p = f.Path.Substring(i);
                sec.Add(new StyledElement(p, () => NavigationController.PushViewController(
                    new SourceInfoController(Username, Slug, Branch, f.Path) { Title = p }, true),
                                          Images.File));
            });

            if (sec.Count == 0)
                sec.Add(new NoItemsElement());

            Root = new RootElement(Title) { sec };
        }
    }
}

