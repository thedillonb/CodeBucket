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

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

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

        public IReactiveCommand<object> GoToHtmlUrlCommand { get; }

        public SourceViewModel(
            string username, string repository, string branch, string path,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            GoToHtmlUrlCommand = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.HtmlUrl)
                .Select(x => !string.IsNullOrEmpty(x)));

            GoToHtmlUrlCommand
                .Select(_ => new WebBrowserViewModel(HtmlUrl))
                .Subscribe(NavigateTo);

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

            var openInCommand = ReactiveCommand.Create(canOpen);
            openInCommand.Subscribe(x => actionMenuService.OpenIn(x, FilePath));

            var shareCommand = ReactiveCommand.Create(canExecute);
            shareCommand.Subscribe(x => actionMenuService.ShareUrl(x, HtmlUrl));

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
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await applicationService.Client.Repositories.GetRawFile(username, repository, branch, path, stream);
                    IsText = true;
                }

                FilePath = filePath;
                HtmlUrl = $"https://bitbucket.org/{username}/{repository}/raw/{branch}/{path.TrimStart('/')}";
            });
        }
    }
}