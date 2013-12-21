using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Filters;
using System;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceTreeViewModel : LoadableViewModel
    {
		private readonly FilterableCollectionViewModel<SourceModel, SourceFilterModel> _content;
        private SourceFilterModel _filter;

		public FilterableCollectionViewModel<SourceModel, SourceFilterModel> Content
        {
            get { return _content; }
        }

		public string Username { get; private set; }

		public string Path { get; private set; }

		public string Branch { get; private set; }

		public string Repository { get; private set; }

        public SourceFilterModel Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
                _content.Refresh();
            }
        }

        public ICommand GoToSourceTreeCommand
        {
			get { return new MvxCommand<SourceModel>(x => ShowViewModel<SourceTreeViewModel>(new NavObject { Username = Username, Branch = Branch, Repository = Repository, Path = x.Path })); }
        }

//        public ICommand GoToSubmoduleCommand
//        {
//			get { return new MvxCommand<SourceModel>(GoToSubmodule);}
//        }

        public ICommand GoToSourceCommand
        {
			get { return new MvxCommand<SourceModel>(x => ShowViewModel<SourceViewModel>(new SourceViewModel.NavObject { Name = x.Name, User = Username, Repository = Repository, Branch = Branch, Path = x.Path }));}
        }

//		private void GoToSubmodule(SourceModel x)
//        {
//            var nameAndSlug = x.GitUrl.Substring(x.GitUrl.IndexOf("/repos/", System.StringComparison.Ordinal) + 7);
//            var repoId = new RepositoryIdentifier(nameAndSlug.Substring(0, nameAndSlug.IndexOf("/git", System.StringComparison.Ordinal)));
//            var sha = x.GitUrl.Substring(x.GitUrl.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
//            ShowViewModel<SourceTreeViewModel>(new NavObject {Username = repoId.Owner, Repository = repoId.Name, Branch = sha});
//        }

        public SourceTreeViewModel()
        {
			_content = new FilterableCollectionViewModel<SourceModel, SourceFilterModel>("SourceViewModel");
            _content.FilteringFunction = FilterModel;
			_content.Bind(x => x.Filter, _content.Refresh);
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Branch = navObject.Branch ?? "master";
            Path = navObject.Path ?? "";
        }

		private IEnumerable<SourceModel> FilterModel(IEnumerable<SourceModel> model)
        {
            var ret = model;
            var order = _content.Filter.OrderBy;
            if (order == SourceFilterModel.Order.Alphabetical)
                ret = model.OrderBy(x => x.Name);
            else if (order == SourceFilterModel.Order.FoldersThenFiles)
                ret = model.OrderBy(x => x.Type).ThenBy(x => x.Name);
            return _content.Filter.Ascending ? ret : ret.Reverse();
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			//return Content.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Branches[Branch].Source[Path].GetInfo(forceCacheInvalidation));
			throw new NotImplementedException();
        }

		public class SourceModel
		{
			public string Name { get; set; }
			public string Type { get; set; }
			public string Path { get; set; }
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public string Path { get; set; }
        }
    }
}

