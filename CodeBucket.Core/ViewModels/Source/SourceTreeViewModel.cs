using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceTreeViewModel : BaseViewModel, ILoadableViewModel
    {
		public string Username { get; private set; }

		public string Path { get; private set; }

		public string Branch { get; private set; }

		public string Repository { get; private set; }

        public CollectionViewModel<SourceModel> Content { get; } = new CollectionViewModel<SourceModel>();

        public IReactiveCommand<object> GoToSourceCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand LoadCommand { get; }


//        public ICommand GoToSubmoduleCommand
//        {
//			get { return new MvxCommand<SourceModel>(GoToSubmodule);}
//        }

//		private void GoToSubmodule(SourceModel x)
//        {
//            var nameAndSlug = x.GitUrl.Substring(x.GitUrl.IndexOf("/repos/", System.StringComparison.Ordinal) + 7);
//            var repoId = new RepositoryIdentifier(nameAndSlug.Substring(0, nameAndSlug.IndexOf("/git", System.StringComparison.Ordinal)));
//            var sha = x.GitUrl.Substring(x.GitUrl.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
//            ShowViewModel<SourceTreeViewModel>(new NavObject {Username = repoId.Owner, Repository = repoId.Name, Branch = sha});
//        }

        public SourceTreeViewModel()
        {
            GoToSourceCommand.OfType<SourceModel>().Subscribe(x =>
            {
                if (x.Type.Equals("dir", StringComparison.OrdinalIgnoreCase))
                {
                    ShowViewModel<SourceTreeViewModel>(new NavObject
                    {
                        Username = Username,
                        Branch = Branch,
                        Repository = Repository,
                        Path = x.Path
                    });
                }
                else if (x.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
                {
                    ShowViewModel<SourceViewModel>(new SourceViewModel.NavObject 
                    { 
                        Name = x.Name, 
                        User = Username,
                        Repository = Repository, 
                        Branch = Branch, 
                        Path = x.Path 
                    });
                }
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var data = await this.GetApplication().Client.Repositories.GetSource(Username, Repository, Branch, Path);
                var dirs = data.Directories.Select(x => new SourceModel { Name = x, Type = "dir", Path = Path + "/" + x });
                var files = data.Files.Select(x => new SourceModel { Name = x.Path.Substring(x.Path.LastIndexOf("/", StringComparison.Ordinal) + 1), Type = "file", Path = x.Path });
                Content.Items.Reset(dirs.Concat(files));
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Branch = navObject.Branch ?? "master";
            Path = navObject.Path ?? "";
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

