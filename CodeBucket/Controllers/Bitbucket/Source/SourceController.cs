using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Filters.Models;

namespace CodeBucket.Bitbucket.Controllers.Source
{
    public class SourceController : BaseListModelController
    {
        private readonly string _username;
        private readonly string _slug;
        private readonly string _branch;
        private readonly string _path;

        public SourceController(string username, string slug, string branch = "master", string path = "")
        {
            _username = username;
            _slug = slug;
            _branch = branch;
            _path = path;
            EnableSearch = true;
            EnableFilter = true;
            SearchPlaceholder = "Search Files & Folders".t();
            Title = string.IsNullOrEmpty(path) ? "Source".t() : path.Substring(path.LastIndexOf('/') + 1);

            var filter = Application.Account.GetFilter(this);
            SetFilterModel(filter != null ? filter.GetData<SourceFilterModel>() : new SourceFilterModel());
        }

        protected override void SaveFilterAsDefault(CodeFramework.Filters.Models.FilterModel model)
        {
            Application.Account.AddFilter(this, model);
        }

        protected override System.Collections.IList OnRenderList()
        {
            var list = base.OnRenderList() as List<object>;
            if (list == null)
                return list;

            var filterModel = GetFilterModel<SourceFilterModel>();
            if (filterModel.OrderBy == (int)SourceFilterModel.Order.Alphabetical)
            {
                list = list.OrderBy(x => {
                    if (x is SourceModel.FileModel)
                    {
                        var path = ((SourceModel.FileModel)x).Path;
                        return path.Substring(path.LastIndexOf('/') + 1);
                    }
                    else
                        return x.ToString();
                }).ToList();
            }
            else if (filterModel.OrderBy == (int)SourceFilterModel.Order.FoldersThenFiles)
            {
                //It's already setup to do folders then files due to our OnUpdateListModel
            }

            return filterModel.Ascending ? list : list.Reverse<object>().ToList();
        }

        protected override object OnUpdateListModel(bool forced, int currentPage, ref int nextPage)
        {
            var sourceModel = Application.Client.Users[_username].Repositories[_slug].Branches[_branch].Source[_path].GetInfo(forced);
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
                return new StyledStringElement(dir, () => NavigationController.PushViewController(new SourceController(_username, _slug, _branch, _path + "/" + dir), true), Images.Folder);
            }
            else if (obj is SourceModel.FileModel)
            {
                var fileModel = (SourceModel.FileModel)obj;
                var i = fileModel.Path.LastIndexOf('/') + 1;
                var p = fileModel.Path.Substring(i);
                return new StyledStringElement(p, () => NavigationController.PushViewController(new SourceInfoController(_username, _slug, _branch, fileModel.Path) { Title = p }, true), Images.File);
            }

            return null;
        }

        protected override CodeFramework.Filters.Controllers.FilterController CreateFilterController()
        {
            return new CodeBucket.Filters.Controllers.SourceFilterController();
        }
    }
}

