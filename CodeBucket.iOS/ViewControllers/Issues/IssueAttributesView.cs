using System;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;

namespace CodeBucket.Views.Issues
{
	public class IssueAttributesView : DialogViewController
    {
		private readonly string[] _values;
		private readonly string _selected;

		public Action<string> SelectedValue;

		public IssueAttributesView(string[] values, string selected)
            : base(UIKit.UITableViewStyle.Plain)
		{
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
                var el = new CheckElement(val);
                el.Checked = string.Equals(val, _selected, StringComparison.OrdinalIgnoreCase);
                el.Clicked.Subscribe(_ =>
                {
                    SelectedValue?.Invoke(capture);
                    NavigationController.PopViewController(true);
                });
				sec.Add(el);
			}

            Root.Reset(sec);
		}

    }
}

