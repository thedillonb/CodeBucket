using System;
using System.Threading.Tasks;
using BitbucketSharp.Models;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;
using CodeBucket.Core.Services;
using System.Text;

namespace CodeBucket.Core.ViewModels.Wiki
{
	public class WikiViewModel : LoadableViewModel
    {
		public string Username { get; set; }
		public string Repository { get; set; }

        private string _page;
        public string Page 
        {
            get { return _page; }
            private set
            {
                _page = value;
                RaisePropertyChanged(() => Page);
            }
        }

		private WikiModel _wiki;
        private WikiModel Wiki
		{
			get { return _wiki; }
			set
			{
				_wiki = value;
				RaisePropertyChanged(() => Wiki);
			}
		}

		private string _contentUrl;
        private string ContentUrl
		{
			get { return _contentUrl; }
			set
			{
				_contentUrl = value;
				RaisePropertyChanged(() => ContentUrl);
			}
		}

        private bool _canEdit;
        private bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                _canEdit = value;
                RaisePropertyChanged(() => CanEdit);
            }
        }

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

		public void Init(NavObject navObject)
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
			Page = navObject.Page ?? "Home";

			if (Page.StartsWith("/", StringComparison.Ordinal))
				Page = Page.Substring(1);

            CanEdit = true;
        }

		protected override Task Load(bool forceCacheInvalidation)
		{
			return this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Wikis[Page].GetInfo(forceCacheInvalidation), x => {
				Wiki = x;

				string content = string.Empty;
				if (string.Equals(x.Markup, "markdown"))
					content = GetService<IMarkdownService>().ConvertMarkdown(x.Data);
				else if (string.Equals(x.Markup, "creole"))
					content = GetService<IMarkdownService>().ConvertCreole(x.Data);
				else if (string.Equals(x.Markup, "textile"))
					content = GetService<IMarkdownService>().ConvertTextile(x.Data);
				else if (string.Equals(x.Markup, "rest"))
				{
					content = x.Data;
				}

                var path = CreateHtmlFile(Page, content);
				path = path.Replace(" ", "%20");
				ContentUrl = "file://" + path + "#" + Environment.TickCount;
			});
		}

        public async Task<string> GetData(string page)
        {
            var x = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Wikis[page].GetInfo(true));

            string content = string.Empty;
            if (string.Equals(x.Markup, "markdown"))
                content = GetService<IMarkdownService>().ConvertMarkdown(x.Data);
            else if (string.Equals(x.Markup, "creole"))
                content = GetService<IMarkdownService>().ConvertCreole(x.Data);
            else if (string.Equals(x.Markup, "textile"))
                content = GetService<IMarkdownService>().ConvertTextile(x.Data);
            else if (string.Equals(x.Markup, "rest"))
            {
                content = x.Data;
            }

            var path = CreateHtmlFile(page, content);
            path = path.Replace(" ", "%20");
            return "file://" + path;
        }


        private string CreateHtmlFile(string title, string data)
		{
			//Generate the markup
			var markup = System.IO.File.ReadAllText("Markdown/markdown.html", Encoding.UTF8);

            var tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), title + ".html");
			using (var tmpStream = new System.IO.FileStream(tmp, System.IO.FileMode.Create))
			{
				var fs = new System.IO.StreamWriter(tmpStream, Encoding.UTF8);
                var titleIndex = markup.IndexOf("{{TITLE}}", StringComparison.Ordinal);
                var dataIndex = markup.IndexOf("{{DATA}}", StringComparison.Ordinal);
                fs.Write(markup.Substring(0, titleIndex));
                fs.Write(title);
                fs.Write(markup.Substring(titleIndex + 9, dataIndex - (titleIndex + 9)));
				fs.Write(data);
				fs.Write(markup.Substring(dataIndex + 8));
				fs.Flush();
			}
			return tmp;
		}

        public string CurrentWikiPage(string request)
        {
            var url = request;
            if (!url.StartsWith("file://", StringComparison.Ordinal))
                return null;
            var s = url.LastIndexOf('/');
            if (s < 0)
                return null;
            if (url.Length < s + 1)
                return null;

            url = url.Substring(s + 1);
            var hashIndex = url.IndexOf("#", StringComparison.Ordinal);
            if (hashIndex > 0)
                url = url.Substring(0, hashIndex);
            return url.Substring(0, url.LastIndexOf(".html", StringComparison.Ordinal)); //Get rid of ".html"
        }

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public string Page { get; set; }
		}
    }
}

