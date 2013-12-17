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
using CodeFramework.Controllers;

namespace CodeBucket.ViewControllers
{
    public class ChangesetDiffViewController : FileSourceViewController
    {
        private string _parent;
        private string _user, _slug, _branch, _path;
        private string _baseText, _newText;
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

            _baseText = JavaScriptStringEncode(oldSource);
            _newText = JavaScriptStringEncode(newSource);
            LoadDiffData();
        }

        protected override bool ShouldStartLoad(NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            var url = request.Url;
            if(url.Scheme.Equals("app")) {
                var func = url.Host;
                if(func.Equals("comment")) {
                    var r = new RestSharp.Deserializers.JsonDeserializer().Deserialize<CreateChangesetCommentModel>(new RestSharp.RestResponse { Content = Decode(url.Fragment) });
                    AddComment(r);
                    return false;
                }
            }

            return base.ShouldStartLoad(request, navigationType);
        }

        private void AddComment(CreateChangesetCommentModel model)
        {
            model.Filename = _path;
            this.DoWork(() => {
                var c = Application.Client.Users[_user].Repositories[_slug].Changesets[_branch].Comments.Create(model.Content, model.LineFrom, model.LineTo, model.ParentId, model.Filename);

                //This will inheriently add it to the controller's comments which we're referencing
                if (Comments != null)
                    Comments.Add(c);

                var a = new List<ChangesetCommentModel>();
                a.Add(c);
                AddComments(a);
            }, (e) => {
                MonoTouch.Utilities.ShowAlert("Unable to Comment", e.Message);
            });
        }

        protected override void DOMReady()
        {
            InvokeOnMainThread(() => Web.EvaluateJavascript("var a = \"" + _baseText + "\"; var b = \"" + _newText + "\"; diff(a, b);"));
            _baseText = _newText = null;
            AddComments(Comments);
        }

        private void AddComments(List<ChangesetCommentModel> comments)
        {
            //Convert it to something light weight
            var slimComments = comments.Where(x => x.Deleted == false && string.Equals(x.Filename, _path)).Select(x => new { 
                Id = x.CommentId, User = x.Username, Avatar = x.UserAvatarUrl, LineTo = x.LineTo, LineFrom = x.LineFrom,
                Content = x.ContentRendered, Date = x.UtcLastUpdated, Parent = x.ParentId
            }).ToList();

            var c = new RestSharp.Serializers.JsonSerializer().Serialize(slimComments);
            BeginInvokeOnMainThread(() => Web.EvaluateJavascript("var a = " + c + "; addComments(a);"));
        }
    }
}

