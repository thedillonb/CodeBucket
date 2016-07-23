using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;
using Splat;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Wiki
{
    public class WikiViewModel : BaseViewModel, ILoadableViewModel
    {
		private Client.V1.Wiki _wiki;
        private Client.V1.Wiki Wiki
		{
			get { return _wiki; }
            set { this.RaiseAndSetIfChanged(ref _wiki, value); }
		}

		private string _content;
        public string Content
		{
            get { return _content; }
            private set { this.RaiseAndSetIfChanged(ref _content, value); }
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

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

        public WikiViewModel(
            string username, string repository, string page = null,
            IMarkdownService markdownService = null, IApplicationService applicationService = null,
            IActionMenuService actionMenuService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            markdownService = markdownService ?? Locator.Current.GetService<IMarkdownService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            page = page ?? "Home";
            CanEdit = true;

            if (page.StartsWith("/", StringComparison.Ordinal))
                page = page.Substring(1);

            Title = page;

            GoToWebCommand
                .OfType<string>()
                .Select(x => string.Format("https://bitbucket.org/{0}/{1}/wiki/{2}", username, repository, x))
                .Select(x => new WebBrowserViewModel(x))
                .Subscribe(NavigateTo);

            GoToPageCommand
                .OfType<string>()
                .Do(x => page = x)
                .InvokeCommand(LoadCommand);

            ShowMenuCommand = ReactiveCommand.CreateAsyncTask(sender =>
            {
                var menu = actionMenuService.Create();
                //menu.AddButton("Fork Repository", ForkCommand);
                menu.AddButton("Show in Bitbucket", () => {
                    var htmlUrl = $"https://bitbucket.org/{username.ToLower()}/{repository.ToLower()}/wiki/{page}";
                    NavigateTo(new WebBrowserViewModel(htmlUrl));
                });
                return menu.Show(sender);
            });

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
                else
                    content = Wiki.Data;

                Content = content;
            });
        }
    }
}

