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
using CodeBucket.ViewControllers;

namespace CodeBucket.Controlleres
{
    public class SourceController : ListController<SourceController.SourceModel, SourceFilterModel>
    {
        private readonly string _username;
        private readonly string _slug;
        private readonly string _branch;
        private readonly string _path;

        public SourceController(IView<ListModel<SourceController.SourceModel>> view, string username, string slug, string branch = "master", string path = "")
            : base(view)
        {
            _username = username;
            _slug = slug;
            _branch = branch;
            _path = path;

            Filter = Application.Account.GetFilter<SourceFilterModel>(this);
        }

        protected override void SaveFilterAsDefault(SourceFilterModel filter)
        {
            Application.Account.AddFilter(this, filter);
        }

        protected override List<SourceModel> FilterModel(List<SourceModel> model, SourceFilterModel filter)
        {
            IEnumerable<SourceModel> ret = model;
            var order = (SourceFilterModel.Order)filter.OrderBy;
            if (order == SourceFilterModel.Order.Alphabetical)
                 ret = model.OrderBy(x => x.Name);
            return filter.Ascending ? ret.ToList() : ret.Reverse().ToList();
        }
  
        public override void Update(bool force)
        {
            var sourceModel = Application.Client.Users[_username].Repositories[_slug].Branches[_branch].Source[_path].GetInfo(force);
            var returnModel = new List<SourceController.SourceModel>();
            foreach (var a in sourceModel.Directories.OrderBy(x => x))
                returnModel.Add(new SourceModel { Name = a });
            foreach (var a in sourceModel.Files)
                returnModel.Add(new SourceModel { Name = a.Path, IsFile = true });
         
            this.Model = new ListModel<SourceController.SourceModel> {
                Data = returnModel
            };
        }

        public class SourceModel
        {
            public string Name { get; set; }
            public bool IsFile { get; set; }
        }
    }
}

