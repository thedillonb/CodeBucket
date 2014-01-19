using System;
using CodeFramework.ViewControllers;
using MonoTouch.Dialog;

namespace CodeBucket.iOS.Views.Issues
{
	public class IssueAttributesView : BaseDialogViewController
    {
		private readonly string[] _values;
		private readonly string _selected;

		public Action<string> SelectedValue;

		public IssueAttributesView(string[] values, string selected)
			: base(true)
		{
			Style = MonoTouch.UIKit.UITableViewStyle.Plain;
			_values = values;
			_selected = selected;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var sec = new Section();
			foreach (var val in _values)
			{
				var capture = val;
				var el = new StyledStringElement(val);
				if (string.Equals(val, _selected, StringComparison.OrdinalIgnoreCase))
					el.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.Checkmark;
				el.Tapped += () => {
					if (SelectedValue != null)
                        SelectedValue(capture);
					NavigationController.PopViewControllerAnimated(true);
				};
				sec.Add(el);
			}

			Root = new RootElement(Title) { sec };
		}

    }
}

