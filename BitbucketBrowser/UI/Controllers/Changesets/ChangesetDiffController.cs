using System;
using System.Text;
using BitbucketBrowser.UI.Controllers.Source;

namespace BitbucketBrowser.UI.Controllers.Changesets
{
    public class ChangesetDiffController : SourceInfoController
    {
        protected string _parent;
        public bool Removed { get; set; }
        public bool Added { get; set; }
        
        public ChangesetDiffController(string user, string slug, string branch, string parent, string path)
            : base(user, slug, branch, path)
        {
            _parent = parent;
        }
        
        protected override string RequestData()
        {
            if (Removed && _parent == null)
            {
                throw new InvalidOperationException("File does not exist!");
            }
            
            
            var newSource = "";
            if (!Removed)
            {
                newSource = System.Security.SecurityElement.Escape(
                    Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path).Data);
            }
            
            var oldSource = "";
            if (_parent != null && !Added)
            {
                try
                {
                    oldSource = System.Security.SecurityElement.Escape(
                        Application.Client.Users[_user].Repositories[_slug].Branches[_parent].Source.GetFile(_path).Data);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to get old source (parent: " + _parent + ") - " + e.Message);
                }
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
            
            return builder.ToString();
        }
    }
}

