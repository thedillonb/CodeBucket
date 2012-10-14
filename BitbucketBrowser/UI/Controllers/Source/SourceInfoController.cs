using System;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using MonoTouch.Foundation;
using CodeFramework.UI.Views;
using BitbucketBrowser.Controllers;
using BitbucketSharp;

namespace BitbucketBrowser.UI.Controllers.Source
{
    public class SourceInfoController : WebViewController
    {
        private static string TempDir = System.IO.Path.Combine(MonoTouch.Utilities.BaseDir, "tmp", "src");
        protected string _user, _slug, _branch, _path;
        private string _tempFile;
        
        public SourceInfoController(string user, string slug, string branch, string path)
            : base(false)
        {
            _user = user;
            _slug = slug;
            _branch = branch;
            _path = path;
            
            Web.DataDetectorTypes = UIDataDetectorType.None;
            
            //Create the filename
            var fileName = System.IO.Path.GetFileName(path);
            if (fileName == null)
                fileName = path.Substring(path.LastIndexOf('/') + 1);
            
            //Create the temp file path
            _tempFile = System.IO.Path.Combine(TempDir, fileName);
            Title = fileName;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Request();
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            
            //Clean up the temporary file!
            try
            {
                if (System.IO.File.Exists(_tempFile))
                    System.IO.File.Delete(_tempFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to delete file: " + e.Message);
            }
        }
        
        protected override void OnLoadError(object sender, UIWebErrorArgs e)
        {
            base.OnLoadError(sender, e);
            
            //Can't load this!
            ErrorView.Show(this.View, "Unable to display this type of file.");
        }
        
        protected virtual void Request()
        {
            this.DoWork(() => {
                
                //There is a bug in the Bitbucket server that says everything returned is text. Content Type: text/plain
                //Attempt to load this the normal way... If we fail then we'll fall back. If that fails then just display an error.
                try 
                {
                    //If this is successful there will be no exception. Just exit out!
                    LoadSourceCode();
                    return;
                }
                catch (InternalServerException ex)
                {
                    Console.WriteLine("Could not grab file the bitbucket way: " + ex.Message);
                }
                
                
                //Attempt to load hte file the raw way.
                LoadRaw();
            }, 
            (ex) => {
                ErrorView.Show(this.View, ex.Message);
            });
        }
        
        private void LoadSourceCode()
        {
            var d = Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFile(_path, true);
            var data = System.Security.SecurityElement.Escape(d.Data);
            
            InvokeOnMainThread(delegate {
                var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
                var filled = html.Replace("{DATA}", data);
                
                var url = NSBundle.MainBundle.BundlePath + "/SourceBrowser";
                url = url.Replace("/", "//").Replace(" ", "%20");
                
                Web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));
            });
        }
        
        private void LoadRaw()
        {
            //Find
            using (var stream = new System.IO.FileStream(_tempFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                Application.Client.Users[_user].Repositories[_slug].Branches[_branch].Source.GetFileRaw(_path, stream);
            }
            
            //Append the tick count because we don't want it to cache anything...
            var uri = Uri.EscapeUriString("file://" + _tempFile) + "#" + Environment.TickCount;
            
            InvokeOnMainThread(delegate {
                Web.LoadRequest(new NSUrlRequest(new NSUrl(uri)));
            });
        }
    }
}

