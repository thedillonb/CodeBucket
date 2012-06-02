using System;
using MonoTouch.Dialog;

using BitbucketSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;


namespace BitbucketBrowser.UI
{
	public class RepositoryInfoController : DialogViewController
	{
		private readonly RepositoryDetailedModel _model;
		public RepositoryInfoController(RepositoryDetailedModel model) 
			: base(null, true)
		{
			Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
			_model = model;
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			Root = new RootElement(_model.Name);
			Root.Add(new Section(new RepoSection(new RectangleF(0, 0, View.Bounds.Width, 60f), this)));
			if (!string.IsNullOrEmpty(_model.Description) && !string.IsNullOrWhiteSpace(_model.Description))
			{
				Root.Add(new Section() {
						new StyledMultilineElement(_model.Description) { Lines = 4, LineBreakMode = UILineBreakMode.WordWrap, Font = UIFont.SystemFontOfSize(14f) }
				});
			}
			Root.Add(new Section() {
						new StringElement("Owner", _model.Owner),
			});
			
		}
		
		private class RepoSection : HeaderView
		{
			private readonly RepositoryInfoController _ctrl;
			private static UIFont NameFont = UIFont.BoldSystemFontOfSize(18);
			private static UIFont FollowFont = UIFont.SystemFontOfSize(12);
			
			public RepoSection(RectangleF rect, RepositoryInfoController ctrl)
				: base(rect)
			{
				_ctrl = ctrl;
			}
			
			public override void Draw(RectangleF rect)
			{
				DrawString(
					_ctrl._model.Name,
					new RectangleF(XPad, YPad, rect.Width - XPad * 2, NameFont.LineHeight),
					NameFont,
					UILineBreakMode.TailTruncation
				);
				
				var s = _ctrl._model.FollowersCount + " followers / " + _ctrl._model.ForkCount + " forks";
				
				UIColor.FromRGB(0.3f, 0.3f, 0.3f).SetColor();
				
				DrawString(
					s,
					new RectangleF(XPad, YPad + NameFont.LineHeight + 2f, rect.Width - XPad * 2, FollowFont.LineHeight),
					FollowFont,
					UILineBreakMode.TailTruncation
				);
			}
			
		}
	}
}

