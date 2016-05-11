using System.IO;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using BitbucketSharp.Models;
using System.Reactive.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceViewModel : FileSourceViewModel, ILoadableViewModel
    {
        private RawFileModel _file;
        public RawFileModel File
        {
            get { return _file; }
            private set { this.RaiseAndSetIfChanged(ref _file, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

        public SourceViewModel(
            string username, string repository, string branch, string path, string name,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            //Create the filename
            var fileName = Path.GetFileName(path);
            if (fileName == null)
                fileName = path.Substring(path.LastIndexOf('/') + 1);

            //Create the temp file path
            Title = fileName;

            var canExecute = this.WhenAnyValue(x => x.File).Select(x => x != null);

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
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(name));
                var file = await applicationService.Client.Repositories.GetFileRaw(username, repository, branch, path);
                IsText = true;

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.Stream.CopyToAsync(stream);
                }

                FilePath = filePath;
            });
        }
    }
}