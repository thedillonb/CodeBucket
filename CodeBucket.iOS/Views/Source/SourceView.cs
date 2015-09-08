using System;
using Foundation;

namespace CodeBucket.Views.Source
{
	public class SourceView : FileSourceView
    {
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.Bind(x => x.FilePath, x =>
            {
                if (ViewModel.IsText)
                {
                    var v = new SourceFileRazorView() { Model = x };
                    LoadContent(v.GenerateString(), System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, "SourceBrowser"));
                }
                else
                {
                    LoadFile(x);
                }
            });
		}
    }
}

