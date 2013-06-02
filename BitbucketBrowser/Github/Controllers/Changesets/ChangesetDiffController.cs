using System;
using System.Text;
using CodeBucket.GitHub.Controllers;
using GitHubSharp;

namespace CodeBucket.GitHub.Controllers.Changesets
{
    public class ChangesetDiffController : FileViewController
    {
        private readonly string _parent;
        private readonly string _user;
        private readonly string _slug;
        private readonly string _branch;
        private readonly string _path;
        public bool Removed { get; set; }
        public bool Added { get; set; }
        
        public ChangesetDiffController(string user, string slug, string branch, string parent, string path)
        {
            _parent = parent;
            _user = user;
            _slug = slug;
            _branch = branch;
            _path = path;

            //Create the filename
            var fileName = System.IO.Path.GetFileName(path) ?? path.Substring(path.LastIndexOf('/') + 1);
            Title = fileName;
        }

        protected override void Request()
        {
            if (Removed && _parent == null)
            {
                throw new InvalidOperationException("File does not exist!");
            }

            try
            {
                RequestSourceDiff();
                return;
            }
            catch (InternalServerException ex)
            {
                Console.WriteLine("Could not generate diff from source. Must be binary: " + ex.Message);
            }

            RequestBinary();
        }

        private void RequestBinary()
        {
            if (!Removed)
            {
                var mime = "";
                var newFile = DownloadFile(_user, _slug, _branch, _path, out mime);
                LoadFile(newFile);
            }
            else if (_parent != null && !Added)
            {
                var mime = "";
                var newFile = DownloadFile(_user, _slug, _parent, _path, out mime);
                LoadFile(newFile);
            }
            else
            {
                throw new InvalidOperationException("Request for file failed.");
            }
        }

        private void RequestSourceDiff()
        {
            var newSource = "";
            if (!Removed)
            {
//                newSource = System.Security.SecurityElement.Escape(
//                    Application.GitHubClient.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path).Data);
            }
            
            var oldSource = "";
            if (_parent != null && !Added)
            {
//                oldSource = System.Security.SecurityElement.Escape(
//                    Application.GitHubClient.Users[_user].Repositories[_slug].Branches[_parent].Source.GetFile(_path).Data);
            }
            
            var differ = new DiffPlex.DiffBuilder.InlineDiffBuilder(new DiffPlex.Differ());
            var a = differ.BuildDiffModel(oldSource, newSource);
            
            var builder = new StringBuilder();
            foreach (var k in a.Lines)
            {
                if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted)
                    builder.Append("<span style='background-color: #ffe0e0;'>" + k.Text + "</span>");
                else if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Inserted)
                    builder.Append("<span style='background-color: #e0ffe0;'>" + k.Text + "</span>");
                else if (k.Type == DiffPlex.DiffBuilder.Model.ChangeType.Modified)
                    builder.Append("<span style='background-color: #ffffe0;'>" + k.Text + "</span>");
                else
                    builder.Append(k.Text);
                
                builder.AppendLine();
            }

            LoadRawData(builder.ToString());
        }
    }
}

