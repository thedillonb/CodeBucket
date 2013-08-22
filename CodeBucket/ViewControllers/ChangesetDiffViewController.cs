using System;
using System.Text;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BitbucketSharp;
using RestSharp.Contrib;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.ViewControllers
{
    public class ChangesetDiffViewController : FileSourceViewController
    {
        private string _parent;
        private string _user, _slug, _branch, _path;
        public bool Removed { get; set; }
        public bool Added { get; set; }
        public List<ChangesetCommentModel> Comments;
        
        public ChangesetDiffViewController(string user, string slug, string branch, string parent, string path)
        {
            _parent = parent;
            _user = user;
            _slug = slug;
            _branch = branch;
            _path = path;

            //Create the filename
            var fileName = System.IO.Path.GetFileName(path);
            if (fileName == null)
                fileName = path.Substring(path.LastIndexOf('/') + 1);
            Title = fileName;
        }

        protected override void Request()
        {
            if (Removed && _parent == null)
            {
                throw new InvalidOperationException("File does not exist!");
            }

            RequestSourceDiff();
        }

        private void RequestSourceDiff()
        {
            var newSource = "";
            var mime = "";
            if (!Removed)
            {
                var file = DownloadFile(_user, _slug, _branch, _path, out mime);
                if (mime.StartsWith("text/plain"))
                    newSource = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                else
                {
                    LoadFile(file);
                    return;
                }
            }
            
            var oldSource = "";
            if (_parent != null && !Added)
            {
                var file = DownloadFile(_user, _slug, _parent, _path, out mime);
                if (mime.StartsWith("text/plain"))
                    oldSource = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                else
                {
                    LoadFile(file);
                    return;
                }
            }

            LoadDiffData(oldSource, newSource);
        }

        int loadCounter = 0;
        bool commentsLoaded = false;
        protected override void OnLoadStarted(object sender, EventArgs e)
        {
            base.OnLoadStarted(sender, e);
            loadCounter++;
        }

        protected override void OnLoadFinished(object sender, EventArgs e)
        {
            base.OnLoadFinished(sender, e);
            loadCounter--;

            if (loadCounter == 0 && commentsLoaded == false)
            {
                //Convert it to something light weight
                var slimComments = Comments.Where(x => x.Deleted == false && string.Equals(x.Filename, _path)).Select(x => new { 
                    Id = x.CommentId, User = x.Username, Avatar = x.UserAvatarUrl, LineTo = x.LineTo, LineFrom = x.LineFrom,
                    Content = x.ContentRendered, Date = x.UtcLastUpdated, Parent = x.ParentId
                }).ToList();

                var comments = new RestSharp.Serializers.JsonSerializer().Serialize(slimComments);
                BeginInvokeOnMainThread(() => Web.EvaluateJavascript("var a = " + comments + "; addComments(a);"));
                commentsLoaded = true;
            }
        }

    }
}

