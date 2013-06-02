using System;
using System.Collections.Generic;
using System.Linq;
using BitbucketBrowser.Data;
using CodeBucket.Controllers;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers.Source
{
    public class SourceController : ListController<ContentModel>
    {
        public string Username { get; private set; }
        public string Slug { get; private set; }
        public string Branch { get; private set; }
        public string Path { get; private set; }

        public SourceController(string username, string slug, string branch = "master", string path = "")
            : base(true)
        {
            Username = username;
            Slug = slug;
            Branch = branch;
            Path = path;
            Title = string.IsNullOrEmpty(path) ? "Source" : path.Substring(path.LastIndexOf('/') + 1);
        }

        protected override Element CreateElement(ContentModel obj)
        {
            if (obj.Type.Equals("dir", StringComparison.OrdinalIgnoreCase))
            {
                return new StyledElement(obj.Name, () => NavigationController.PushViewController(
                       new SourceController(Username, Slug, Branch, obj.Path), true), Images.Folder);
            }
            else if (obj.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                return new StyledElement(obj.Name, () => NavigationController.PushViewController(
                       new SourceInfoController(Username, Slug, Branch, obj.Path) { Title = obj.Name }, true), Images.File);
            }
            else
            {
                return new StyledElement(obj.Name, Images.File);
            }
        }

        protected override List<ContentModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var d = Application.GitHubClient.API.GetRepositoryContent(Username, Slug, Path, Branch);
            nextPage = -1;
            return d.Data.OrderBy(x => x.Name).GroupBy(x => x.Type).OrderBy(a => a.Key).SelectMany(a => a).ToList();
        }
    }
}


