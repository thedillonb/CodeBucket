using System;
using System.IO;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using BitbucketSharp.Models;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceViewModel : FileSourceViewModel, ILoadableViewModel
    {
		private string _user;
		private string _repository;
		private string _branch;
		private string _path;
		private string _name;

        private RawFileModel _file;
        public RawFileModel File
        {
            get { return _file; }
            private set
            {
                if (_file != value)
                    return;
                
                _file = value;
                RaisePropertyChanged();
            }
        }

        public IReactiveCommand LoadCommand { get; }

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

        public SourceViewModel(IApplicationService applicationService, IActionMenuService actionMenuService)
        {
            var canExecute = this.Bind(x => x.File).Select(x => x != null);

            var openInCommand = ReactiveCommand.Create()
                .WithSubscription(x => actionMenuService.OpenIn(x, null));

            var shareCommand = ReactiveCommand.Create(canExecute)
                .WithSubscription(x => actionMenuService.ShareUrl(x, File.HtmlUrl));

            var showInCommand = ReactiveCommand.Create();

            ShowMenuCommand = ReactiveCommand.CreateAsyncTask(canExecute, sender =>
            {
                var menu = actionMenuService.Create();
                menu.AddButton("Open In", openInCommand);
                menu.AddButton("Share", shareCommand);
                menu.AddButton("Show in Bitbucket", showInCommand);
                return menu.Show(sender);
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(_name));
                var file = await applicationService.Client.Repositories.GetFileRaw(_user, _repository, _branch, _path);
                IsText = true;

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.Stream.CopyToAsync(stream);
                }

                FilePath = filePath;
            });
        }

		public void Init(NavObject navObject)
		{
			_path = navObject.Path;
			_name = navObject.Name;
			_user = navObject.User;
			_repository = navObject.Repository;
			_branch = navObject.Branch;

			//Create the filename
			var fileName = System.IO.Path.GetFileName(_path);
			if (fileName == null)
				fileName = _path.Substring(_path.LastIndexOf('/') + 1);

			//Create the temp file path
			Title = fileName;
		}

		public class NavObject
		{
			public string Path { get; set; }
			public string Name { get; set; }
			public string User { get; set; }
			public string Repository { get; set; }
			public string Branch { get; set; }
		}
    }
}