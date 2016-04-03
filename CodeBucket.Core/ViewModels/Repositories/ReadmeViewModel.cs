using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using CodeBucket.Core.Services;
using BitbucketSharp.Models;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class ReadmeViewModel : LoadableViewModel
    {
        private readonly IMarkdownService _markdownService;
        private readonly IApplicationService _applicationService;
        private string _htmlUrl;

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public string Filename { get; private set; }

        private FileModel _contentModel;
        public FileModel ContentModel
        {
            get { return _contentModel; }
            private set { this.RaiseAndSetIfChanged(ref _contentModel, value); }
        }

        private string _contentText;
        public string ContentText
        {
            get { return _contentText; }
            private set { this.RaiseAndSetIfChanged(ref _contentText, value); }
        }

        public ReactiveUI.IReactiveCommand ShowMenuCommand { get; }

        public ReadmeViewModel(
            IApplicationService applicationService, 
            IMarkdownService markdownService,
            IActionMenuService actionMenuService)
        {
            _applicationService = applicationService;
            _markdownService = markdownService;

            var canShowMenu = this.Bind(x => x.ContentModel, true).Select(x => x != null);

            var gotoCommand = ReactiveUI.ReactiveCommand.Create(canShowMenu);
            gotoCommand.Subscribe(_ => GoToUrlCommand.Execute(_htmlUrl));

            ShowMenuCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(canShowMenu, sender => {
                var shareCommand = ReactiveUI.ReactiveCommand.Create();
                shareCommand.Subscribe(_ => actionMenuService.ShareUrl(sender, new Uri(_htmlUrl)));

                var menu = actionMenuService.Create();
                menu.AddButton("Share", shareCommand);
                menu.AddButton("Show in Bitbucket", gotoCommand);
                return menu.Show(sender);
            });
        }

        protected override async Task Load()
        {
            var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Filename);
            var mainBranch = (await _applicationService.Client.Repositories.GetMainBranch(Username, Repository)).Name;
            ContentModel = await _applicationService.Client.Repositories.GetFile(Username, Repository, mainBranch, Filename);

            var readme = ContentModel.Data;
            _htmlUrl = "http://bitbucket.org/" + Username + "/" + Repository + "/src/" + mainBranch + "/" + Filename;

            if (filepath.EndsWith("textile", StringComparison.Ordinal))
                ContentText = _markdownService.ConvertTextile(readme);
            else
                ContentText = _markdownService.ConvertMarkdown(readme);
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Filename = navObject.Filename;
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Filename { get; set; }
        }
    }
}
