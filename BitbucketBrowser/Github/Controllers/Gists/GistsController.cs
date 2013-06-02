using System;
using CodeBucket.Controllers;
using GitHubSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeBucket.Data;
using CodeBucket.Elements;

namespace CodeBucket.GitHub.Controllers.Gists
{
    public class GistsController : ListController<GistModel>
    {
        public string User { get; private set; }

        public GistsController(string user, bool push = true)
            : base(push)
        {
            User = user;
            Title = "Gists";
            UnevenRows = true;
        }

        protected override List<GistModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var a = Application.GitHubClient.API.GetGists(User, currentPage);
            nextPage = a.Next == null ? -1 : currentPage + 1;
            return a.Data;
        }

        protected override Element CreateElement(GistModel x)
        {
            var str = string.IsNullOrEmpty(x.Description) ? "Gist " + x.Id : x.Description;
            var sse = new NameTimeStringElement() { 
                Time = x.UpdatedAt, 
                String = str, 
                Lines = 4, 
                Image = Images.Anonymous,
            };

            sse.Name = (x.User == null) ? "Anonymous" : x.User.Login;
            sse.ImageUri = (x.User == null) ? null : new Uri(x.User.AvatarUrl);
            sse.Tapped += () => NavigationController.PushViewController(new GistInfoController(x.Id) { Model = x }, true);
            return sse;
        }
    }
}

