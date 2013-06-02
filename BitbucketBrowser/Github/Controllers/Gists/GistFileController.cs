using BitbucketBrowser;
using GitHubSharp.Models;

namespace CodeBucket.GitHub.Controllers.Gists
{
    public class GistFileController : FileViewController
    {
        private string _url;
        public GistFileController(GistFileModel model)
        {
            _url = model.RawUrl;
            Title = model.Filename;
        }

        protected override void Request()
        {
            var data = Application.GitHubClient.API.GetGistFile(_url);
            LoadRawData(data);
        }
    }
}

