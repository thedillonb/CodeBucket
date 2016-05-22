using System.IO;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceViewModel : FileSourceViewModel, ILoadableViewModel
    {
        private static readonly string[] MarkdownExtensions = { ".markdown", ".mdown", ".mkdn", ".md", ".mkd", ".mdwn", ".mdtxt", ".mdtext", ".text" };

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

        public bool IsMarkdown { get; }

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

            var extension = Path.GetExtension(path);
            IsMarkdown = MarkdownExtensions.Any(x => x == extension);

            var canExecute = this.WhenAnyValue(x => x.HtmlUrl).Select(x => x != null);
            var canOpen = this.WhenAnyValue(x => x.FilePath).Select(x => x != null);

            var openInCommand = ReactiveCommand.Create(canOpen)
                .WithSubscription(x => actionMenuService.OpenIn(x, FilePath));

            var shareCommand = ReactiveCommand.Create(canExecute)
                .WithSubscription(x => actionMenuService.ShareUrl(x, HtmlUrl));

            var canShow = Observable.CombineLatest(canExecute, canOpen, (x, y) => x && y);
            ShowMenuCommand = ReactiveCommand.CreateAsyncTask(canShow, sender =>
            {
                var menu = actionMenuService.Create();
                menu.AddButton("Open In", openInCommand);
                menu.AddButton("Share", shareCommand);
                menu.AddButton("Show in Bitbucket", GoToHtmlUrlCommand);
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
                HtmlUrl = file.HtmlUrl;
            });
        }
    }
}