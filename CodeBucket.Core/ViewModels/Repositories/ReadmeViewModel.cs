using System;
using CodeBucket.Core.Services;
using BitbucketSharp.Models;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class ReadmeViewModel : BaseViewModel, ILoadableViewModel
    {
        private string _htmlUrl;

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

        public IReactiveCommand ShowMenuCommand { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public ReadmeViewModel(
            string username, string repository, string filename,
            IApplicationService applicationService = null, 
            IMarkdownService markdownService = null,
            IActionMenuService actionMenuService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            markdownService = markdownService ?? Locator.Current.GetService<IMarkdownService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            var canShowMenu = this.WhenAnyValue(x => x.ContentModel).Select(x => x != null);

            var gotoCommand = ReactiveCommand.Create(canShowMenu);
            gotoCommand
                .Select(_ => new WebBrowserViewModel(_htmlUrl))
                .Subscribe(NavigateTo);

            ShowMenuCommand = ReactiveCommand.CreateAsyncTask(canShowMenu, sender => 
            {
                var shareCommand = ReactiveCommand.Create();
                shareCommand.Subscribe(_ => actionMenuService.ShareUrl(sender, new Uri(_htmlUrl)));

                var menu = actionMenuService.Create();
                menu.AddButton("Share", shareCommand);
                menu.AddButton("Show in Bitbucket", gotoCommand);
                return menu.Show(sender);
            });

            Title = "Readme";

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
                var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), filename);
                var mainBranch = (await applicationService.Client.Repositories.GetMainBranch(username, repository)).Name;
                ContentModel = await applicationService.Client.Repositories.GetFile(username, repository, mainBranch, filename);

                var readme = ContentModel.Data;
                _htmlUrl = "http://bitbucket.org/" + username + "/" + repository + "/src/" + mainBranch + "/" + filename;

                if (filepath.EndsWith("textile", StringComparison.Ordinal))
                    ContentText = markdownService.ConvertTextile(readme);
                else
                    ContentText = markdownService.ConvertMarkdown(readme);
            });
        }
    }
}
