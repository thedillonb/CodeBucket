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

            ViewModel.Bind(x => x.FilePath, true).IsNotNull().Subscribe(x =>
            {
                if (ViewModel.IsText)
                {
                    var content = System.IO.File.ReadAllText(x, System.Text.Encoding.UTF8);
                    var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
                    var model = new SourceBrowserModel(content, "idea", fontSize);
                    var v = new SyntaxHighlighterView { Model = model };
                    LoadContent(v.GenerateString());
                }
                else
                {
                    LoadFile(x);
                }
            });
		}
    }
}

