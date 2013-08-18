using System;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeBucket.Controllers;
using System.Text;
using CodeFramework.Controllers;
using CodeFramework.Views;

namespace CodeBucket.ViewControllers
{
    public abstract class FileSourceViewController : CodeFramework.Controllers.FileSourceController
    {
        protected static string DownloadFile(string user, string slug, string branch, string path, out string mime)
        {
            //Create a temporary filename
            var ext = System.IO.Path.GetExtension(path);
            if (ext == null) ext = string.Empty;
            var filename = Environment.TickCount + ext;
            
            var filepath = System.IO.Path.Combine(TempDir, filename);
            
            //Find
            using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var response = Application.Client.Users[user].Repositories[slug].Branches[branch].Source.GetFileRaw(path, stream);
                mime = response.ContentType;
            }
            
            return filepath;
        }
    }
}

