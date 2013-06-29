using System;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeBucket.Controllers;
using System.Text;

namespace CodeBucket.Bitbucket.Controllers
{
    public abstract class FileSourceController : WebViewController
    {
        protected static string TempDir = System.IO.Path.Combine(MonoTouch.Utilities.BaseDir, "tmp", "source");
        
        public FileSourceController()
            : base(false)
        {
            Web.DataDetectorTypes = UIDataDetectorType.None;
            Title = "Source";
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //Create the temp directory if it does not exist!
            if (!System.IO.Directory.Exists(TempDir))
                System.IO.Directory.CreateDirectory(TempDir);

            //Do the request
            this.DoWork(Request, ex => ErrorView.Show(this.View, ex.Message));
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            //Remove all files within the temp directory
            if (System.IO.Directory.Exists(TempDir))
                System.IO.Directory.Delete(TempDir, true);
        }
        
        protected override void OnLoadError(object sender, UIWebErrorArgs e)
        {
            base.OnLoadError(sender, e);
            
            //Can't load this!
            ErrorView.Show(this.View, "Unable to display this type of file.");
        }
        
        protected abstract void Request();

        protected void LoadRawData(string data)
        {
            InvokeOnMainThread(delegate {
                var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
                var filled = html.Replace("{DATA}", data);
                
                var url = NSBundle.MainBundle.BundlePath + "/SourceBrowser";
                url = url.Replace("/", "//").Replace(" ", "%20");
                
                Web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));
            });
        }

        protected void LoadFile(string path)
        {
            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(uri))));
        }
        
        protected static string DownloadFile(string user, string slug, string branch, string path)
        {
            //Create a temporary filename
            var ext = System.IO.Path.GetExtension(path);
            if (ext == null) ext = string.Empty;
            var filename = Environment.TickCount + ext;
            
            var filepath = System.IO.Path.Combine(TempDir, filename);
            
            //Find
            using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                Application.Client.Users[user].Repositories[slug].Branches[branch].Source.GetFileRaw(path, stream);
            }
            
            return filepath;
        }
    }
}

