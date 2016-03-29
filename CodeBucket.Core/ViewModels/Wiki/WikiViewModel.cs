using System;
using BitbucketSharp.Models;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Wiki
{
    public class WikiViewModel : BaseViewModel, ILoadableViewModel
    {
		public string Username { get; set; }
		public string Repository { get; set; }

        private string _page;
        public string Page 
        {
            get { return _page; }
            set { this.RaiseAndSetIfChanged(ref _page, value); }
        }

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

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        private ICommand GoToPageCommand
		{
			get 
			{ 
				return new MvxCommand<string>(x => 
				{
					Page = x;
					LoadCommand.Execute(true);
				}); 
			}
		}

        public ICommand GoToWebCommand
        {
            get
            {
                return new MvxCommand<string>(x =>
                {
                    var url = string.Format("https://bitbucket.org/{0}/{1}/wiki/{2}", Username, Repository, x);
                    GoToUrlCommand.Execute(url);
                });
            }
        }

        public WikiViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Wiki = await applicationService.Client.Repositories.GetWiki(Username, Repository, Page);

                string content = string.Empty;
                if (string.Equals(Wiki.Markup, "markdown"))
                    content = GetService<IMarkdownService>().ConvertMarkdown(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "creole"))
                    content = GetService<IMarkdownService>().ConvertCreole(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "textile"))
                    content = GetService<IMarkdownService>().ConvertTextile(Wiki.Data);
                else if (string.Equals(Wiki.Markup, "rest"))
                {
                    content = Wiki.Data;
                }

                Content = content;
            });
        }

		public void Init(NavObject navObject)
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
			Page = navObject.Page ?? "Home";

			if (Page.StartsWith("/", StringComparison.Ordinal))
				Page = Page.Substring(1);

            CanEdit = true;
        }

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public string Page { get; set; }
		}
    }
}

