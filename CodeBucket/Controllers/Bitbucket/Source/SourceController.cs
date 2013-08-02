using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Bitbucket.Controllers.Source
{
    public class SourceController : BaseListModelController
    {
        public string Username { get; private set; }

        public string Slug { get; private set; }

        public string Branch { get; private set; }

        public string Path { get; private set; }

        public SourceController(string username, string slug, string branch = "master", string path = "")
            : base(typeof(List<object>))
        {
            Username = username;
            Slug = slug;
            Branch = branch;
            Path = path;
            EnableSearch = true;
            SearchPlaceholder = "Search Files & Folders";
            Title = string.IsNullOrEmpty(path) ? "Source" : path.Substring(path.LastIndexOf('/') + 1);
        }

        protected override object OnUpdateListModel(bool forced, int currentPage, ref int nextPage)
        {
            var sourceModel = Application.Client.Users[Username].Repositories[Slug].Branches[Branch].Source[Path].GetInfo(forced);
            var returnModel = new List<object>();
            foreach (var a in sourceModel.Directories.OrderBy(x => x))
                returnModel.Add(a);
            foreach (var a in sourceModel.Files)
                returnModel.Add(a);
            return returnModel;
        }

        protected override Element CreateElement(object obj)
        {
            if (obj is string)
            {
                var dir = obj.ToString();
                return new StyledElement(dir, () => NavigationController.PushViewController(new SourceController(Username, Slug, Branch, Path + "/" + dir), true), Images.Folder);
            }
            else if (obj is SourceModel.FileModel)
            {
                var fileModel = (SourceModel.FileModel)obj;
                var i = fileModel.Path.LastIndexOf('/') + 1;
                var p = fileModel.Path.Substring(i);
                return new StyledElement(p, () => NavigationController.PushViewController(new SourceInfoController(Username, Slug, Branch, fileModel.Path) { Title = p }, true), Images.File);
            }

            return null;
        }
    }
}

