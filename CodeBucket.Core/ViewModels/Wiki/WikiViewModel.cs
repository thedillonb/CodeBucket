using System;
using CodeFramework.Core.ViewModels;
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
		public string Page { get; set; }

		private WikiModel _wiki;
		public WikiModel Wiki
		{
			get { return _wiki; }
			private set
			{
				_wiki = value;
				RaisePropertyChanged(() => Wiki);
			}
		}

		private string _contentUrl;
		public string ContentUrl
		{
			get { return _contentUrl; }
			private set
			{
				_contentUrl = value;
				RaisePropertyChanged(() => ContentUrl);
			}
		}

		public ICommand GoToEditCommand
		{
			get { return new MvxCommand(null); }
		}

		public ICommand GoToPageCommand
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

		public void Init(NavObject navObject)
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
			Page = navObject.Page ?? "Home";

			if (Page.StartsWith("/", StringComparison.Ordinal))
				Page = Page.Substring(1);
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

				var path = CreateHtmlFile(content);
				path = path.Replace(" ", "%20");
				ContentUrl = "file://" + path + "#" + Environment.TickCount;
			});
		}


		private string CreateHtmlFile(string data)
		{
			//Generate the markup
			var markup = System.IO.File.ReadAllText("Markdown/markdown.html", Encoding.UTF8);

			var tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName() + ".html");
			using (var tmpStream = new System.IO.FileStream(tmp, System.IO.FileMode.Create))
			{
				var fs = new System.IO.StreamWriter(tmpStream, Encoding.UTF8);
				var dataIndex = markup.IndexOf("{{DATA}}", StringComparison.Ordinal);
				fs.Write(markup.Substring(0, dataIndex));
				fs.Write(data);
				fs.Write(markup.Substring(dataIndex + 8));
				fs.Flush();
			}
			return tmp;
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public string Page { get; set; }
		}
    }
}

