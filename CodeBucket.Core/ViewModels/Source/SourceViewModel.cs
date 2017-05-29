using System;
using System.IO;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Splat;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceViewModel : BaseViewModel, ILoadableViewModel
    {
        private static readonly string[] MarkdownExtensions = { ".markdown", ".mdown", ".mkdn", ".md", ".mkd", ".mdwn", ".mdtxt", ".mdtext", ".text" };

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<object, Unit> ShowMenuCommand { get; }

        public bool IsMarkdown { get; }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            protected set { this.RaiseAndSetIfChanged(ref _filePath, value); }
        }

        private bool _isText;
        public bool IsText
        {
            get { return _isText; }
            protected set { this.RaiseAndSetIfChanged(ref _isText, value); }
        }

        private string _htmlUrl;
        public string HtmlUrl
        {
            get { return _htmlUrl; }
            protected set { this.RaiseAndSetIfChanged(ref _htmlUrl, value); }
        }

        public ReactiveCommand<Unit, Unit> GoToHtmlUrlCommand { get; }

        public SourceViewModel(
            string username, string repository, string branch, string path,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            GoToHtmlUrlCommand = ReactiveCommand.Create(
                () => NavigateTo(new WebBrowserViewModel(HtmlUrl)),
                this.WhenAnyValue(x => x.HtmlUrl).Select(x => !string.IsNullOrEmpty(x)));

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

            var canShow = Observable.CombineLatest(canExecute, canOpen, (x, y) => x && y);
            ShowMenuCommand = ReactiveCommand.CreateFromTask<object>(sender =>
            {
                var menu = actionMenuService.Create();
                menu.AddButton("Open In", x => actionMenuService.OpenIn(x, FilePath));
                menu.AddButton("Share", x => actionMenuService.ShareUrl(x, HtmlUrl));
                menu.AddButton("Show in Bitbucket", _ => GoToHtmlUrlCommand.ExecuteNow());
                return menu.Show(sender);
            }, canShow);

            LoadCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await applicationService.Client.Repositories.GetRawFile(username, repository, branch, path, stream);
                }

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var read = stream.Read(buffer, 0, 1024);
                    IsText = !buffer.Take(read).Any(x => x == 0);
                }

                FilePath = filePath;
                HtmlUrl = $"https://bitbucket.org/{username}/{repository}/raw/{branch}/{path.TrimStart('/')}";
            });
        }
    }
}