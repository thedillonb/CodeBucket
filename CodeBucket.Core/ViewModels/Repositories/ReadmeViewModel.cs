using System;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class ReadmeViewModel : LoadableViewModel
    {
        private readonly IMarkdownService _markdownService;
        private string _data;
        private string _path;
        private string _htmlUrl;

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public string Branch { get; private set; }

        public string Filename { get; private set; }

        public string Data
        {
            get { return _data; }
            set { _data = value; RaisePropertyChanged(() => Data); }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; RaisePropertyChanged(() => Path); }
        }

        public ICommand GoToGitHubCommand
        {
            get { return new MvxCommand(() => GoToUrlCommand.Execute(_htmlUrl), () => _htmlUrl != null); }
        }

        public ICommand GoToLinkCommand
        {
            get { return GoToUrlCommand; }
        }

        public new ICommand ShareCommand
        {
            get
            {
                return new MvxCommand(() => GetService<IShareService>().ShareUrl(_htmlUrl), () => _htmlUrl != null);
            }
        }

        public ReadmeViewModel(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        protected override async Task Load(bool forceCacheInvalidation)
        {
            var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Filename);
            var source = this.GetApplication().Client.Users[Username].Repositories[Repository].Branches[Branch].Source;
            _htmlUrl = "http://bitbucket.org/" + source.Branch.Branches.Repository.Owner.Username + "/" + source.Branch.Branches.Repository.Slug + "/src/" + source.Branch.UrlSafeName + "/" + Filename;
            var file = await Task.Run(() => source.GetFile(Filename));
            string readme = file.Data;
            string data;
            if (filepath.EndsWith("textile", StringComparison.Ordinal))
                data = _markdownService.ConvertTextile(readme);
            else
                data = _markdownService.ConvertMarkdown(readme);
            Path = CreateHtmlFile(data);
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

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Branch = navObject.Branch;
            Filename = navObject.Filename;
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
            public string Filename { get; set; }
        }
    }
}
