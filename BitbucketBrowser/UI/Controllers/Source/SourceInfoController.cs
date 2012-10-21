using System;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using MonoTouch.Foundation;
using CodeFramework.UI.Views;
using BitbucketSharp;

namespace BitbucketBrowser.UI.Controllers.Source
{
    public class SourceInfoController : WebViewController
    {
        private static readonly string TempDir = System.IO.Path.Combine(MonoTouch.Utilities.BaseDir, "tmp", "src");
        protected string User, Slug, Branch, Path;
        private readonly string _tempFile;

        public SourceInfoController(string user, string slug, string branch, string path)
            : base(false)
        {
            User = user;
            Slug = slug;
            Branch = branch;
            Path = path;

            Web.DataDetectorTypes = UIDataDetectorType.None;

            //Create the filename
            var fileName = System.IO.Path.GetFileName(path) ?? path.Substring(path.LastIndexOf('/') + 1);

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
            ErrorView.Show(View, "Unable to display this type of file.");
        }

        protected virtual void Request()
        {
            this.DoWork(() =>
            {

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
            ex => ErrorView.Show(View, ex.Message));
        }

        private void LoadSourceCode()
        {
            var d = Application.Client.Users[User].Repositories[Slug].Branches[Branch].Source.GetFile(Path, true);
            var data = System.Security.SecurityElement.Escape(d.Data);

            InvokeOnMainThread(delegate
            {
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
                Application.Client.Users[User].Repositories[Slug].Branches[Branch].Source.GetFileRaw(Path, stream);
            }

            //Append the tick count because we don't want it to cache anything...
            var uri = Uri.EscapeUriString("file://" + _tempFile) + "#" + Environment.TickCount;

            InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(uri))));
        }
    }
}

