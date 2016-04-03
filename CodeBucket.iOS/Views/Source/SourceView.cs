using System;
using UIKit;
using System.Reactive.Linq;

namespace CodeBucket.Views.Source
{
	public class SourceView : FileSourceView
    {
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.Bind(x => x.FilePath, true)
                .IsNotNull()
                .Subscribe(Load);
		}

        private void Load(string path)
        {
            if (ViewModel.IsText)
            {
                var content = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
                var zoom = UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Phone;
                var model = new SourceBrowserModel(content, "idea", fontSize, zoom, path);
                var v = new SyntaxHighlighterView { Model = model };
                LoadContent(v.GenerateString());
            }
            else
            {
                LoadFile(path);
            }
        }
    }
}

