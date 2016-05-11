using System;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using Splat;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Wiki
{
    public class WikiViewModel : BaseViewModel, ILoadableViewModel
    {
		private WikiModel _wiki;
        private WikiModel Wiki
		{
			get { return _wiki; }
            set { this.RaiseAndSetIfChanged(ref _wiki, value); }
		}

		private string _content;
        private string Content
		{
            get { return _content; }
            set { this.RaiseAndSetIfChanged(ref _content, value); }
		}

        private bool _canEdit;
        private bool CanEdit
        {
            get { return _canEdit; }
            set { this.RaiseAndSetIfChanged(ref _canEdit, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private IReactiveCommand<object> GoToPageCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToWebCommand { get; } = ReactiveCommand.Create();

        public WikiViewModel(
            string username, string repository, string page = null,
            IMarkdownService markdownService = null, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            markdownService = markdownService ?? Locator.Current.GetService<IMarkdownService>();

            page = page ?? "Home";
            CanEdit = true;

            if (page.StartsWith("/", StringComparison.Ordinal))
                page = page.Substring(1);

            GoToWebCommand
                .OfType<string>()
                .Select(x => string.Format("https://bitbucket.org/{0}/{1}/wiki/{2}", username, repository, x))
                .Select(x => new WebBrowserViewModel(x))
                .Subscribe(NavigateTo);

            GoToPageCommand
                .OfType<string>()
                .Do(x => page = x)
                .InvokeCommand(LoadCommand);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Wiki = await applicationService.Client.Repositories.GetWiki(username, repository, page);

                string content = string.Empty;
                if (string.Equals(Wiki.Markup, "markdown"))
                    content = markdownService.ConvertMarkdown(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "creole"))
                    content = markdownService.ConvertCreole(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "textile"))
                    content = markdownService.ConvertTextile(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "rest"))
                {
                    content = Wiki.Data;
                }

                Content = content;
            });
        }
    }
}

